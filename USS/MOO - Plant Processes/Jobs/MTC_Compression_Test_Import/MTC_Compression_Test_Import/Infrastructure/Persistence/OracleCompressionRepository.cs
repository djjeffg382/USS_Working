using System;
using System.Linq;
using System.Threading.Tasks;
using MOO.DAL.ToLive.Models;
using MOO.DAL.ToLive.Services;
using MTC_Compression_Test_Import.Domain.Entities;
using MTC_Compression_Test_Import.Domain.Interfaces;
using Oracle.ManagedDataAccess.Client;
using Serilog;

namespace MTC_Compression_Test_Import.Infrastructure.Persistence;

public sealed class OracleCompressionRepository : ICompressionRepository
{
    private readonly string _connectionString;
    private readonly bool _previewDatabaseWrites;

    public OracleCompressionRepository(bool previewDatabaseWrites)
    {
        _connectionString = MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART);
        _previewDatabaseWrites = previewDatabaseWrites;
    }

    public async Task<int> InsertCompressionBatchAsync(CompressionParseResult result)
    {
        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (_previewDatabaseWrites)
        {
            PreviewInsertCompressionBatch(result);
            return 0;
        }

        using var conn = new OracleConnection(_connectionString);
        await conn.OpenAsync().ConfigureAwait(false);
        using var trans = conn.BeginTransaction();

        try
        {
            var header = new Lab_Compression
            {
                Test_Nbr = Convert.ToInt16(result.TestNumber),
                Instrument = (short)result.InstrumentId
            };

            await Lab_CompressionSvc.InsertAsync(header, conn).ConfigureAwait(false);

            foreach (var row in result.DataPoints)
            {
                var detail = new Lab_Compression_Dtl
                {
                    Comp_Date = row.Timestamp,
                    Comp_Lbs = row.CompressionLbs,
                    Pellet_Nbr = (short)row.PelletNumber,
                    Comp_Id = header.Comp_Id
                };

                await Lab_Compression_DtlSvc.InsertAsync(detail, conn).ConfigureAwait(false);
            }

            trans.Commit();
            return header.Comp_Id;
        }
        catch
        {
            try
            {
                trans.Rollback();
            }
            catch
            {
                // ignore rollback errors
            }

            throw;
        }
    }

    public async Task<CompressionStats> UpdateStdDevAsync(int compId)
    {
        if (_previewDatabaseWrites)
        {
            PreviewUpdateStdDev(compId);
            return new CompressionStats(0, 0, 0, 0);
        }

        var details = await Lab_Compression_DtlSvc.GetByLabCompressionIdAsync(compId).ConfigureAwait(false);

        var qualifying = details
            .Where(d => d.Comp_Lbs > 90)
            .ToList();

        if (qualifying.Count == 0)
        {
            throw new InvalidOperationException($"No compression rows found for comp_id {compId}.");
        }

        var n = qualifying.Count;
        var sum = qualifying.Sum(d => d.Comp_Lbs);
        var sumSq = qualifying.Sum(d => d.Comp_Lbs * d.Comp_Lbs);

        var average = sum / n;

        double variance;
        if (n > 1)
        {
            variance = (sumSq - (sum * sum) / n) / (n - 1);
        }
        else
        {
            variance = 0;
        }

        var stddev = Math.Sqrt(variance);

        if (stddev <= 0)
        {
            throw new InvalidOperationException($"Standard deviation is {stddev} for comp_id {compId}.");
        }

        var comp200 = ComputeCompValue(average, stddev, 200);
        var comp300 = ComputeCompValue(average, stddev, 300);

        var header = await Lab_CompressionSvc.GetAsync(compId).ConfigureAwait(false);
        if (header is null)
        {
            throw new InvalidOperationException($"No lab_compression row found for comp_id {compId}.");
        }

        // Round only when storing, to mimic historical output formatting
        header.Comp200 = Math.Round(comp200, 3);
        header.Comp300 = Math.Round(comp300, 3);
        header.Average = Math.Round(average, 3);

        await Lab_CompressionSvc.UpdateAsync(header).ConfigureAwait(false);

        return new CompressionStats(average, stddev, comp200, comp300);
    }

    private void PreviewInsertCompressionBatch(CompressionParseResult result)
    {
        var testNbr = Convert.ToInt16(result.TestNumber);
        var instrument = (short)result.InstrumentId;

        // Preview header insert
        Log.Information(
            "[DB PREVIEW] INSERT into LAB_COMPRESSION (TEST_NBR, INSTRUMENT) VALUES ({TestNbr}, {Instrument})",
            testNbr,
            instrument);

        foreach (var row in result.DataPoints)
        {
            Log.Information(
                "[DB PREVIEW] INSERT into LAB_COMPRESSION_DTL (COMP_DATE, COMP_LBS, PELLET_NBR, COMP_ID=<new>) VALUES ({CompDate:o}, {CompLbs}, {PelletNbr})",
                row.Timestamp,
                row.CompressionLbs,
                (short)row.PelletNumber);
        }
    }

    private void PreviewUpdateStdDev(int compId)
    {
        Log.Information(
            "[DB PREVIEW] UPDATE LAB_COMPRESSION SET AVERAGE=<computed>, STDDEV=<computed>, COMP200=<computed>, COMP300=<computed> WHERE COMP_ID={CompId}",
            compId);
    }

    private static double ComputeCompValue(double average, double stddev, double target)
    {
        var z = (average - target) / stddev;
        const double b1 = 0.31938153;
        const double b2 = -0.356563872;
        const double b3 = 1.781477937;
        const double b4 = -1.821255978;
        const double b5 = 1.330274429;
        var b6 = 2 * Math.PI;

        var fj = (1 / Math.Sqrt(b6)) * (1 / Math.Pow(Math.E, (z * z) / 2));
        var t = 1 / (1 + 0.2316419 * z);
        var comp = fj * (b1 * t + b2 * Math.Pow(t, 2) + b3 * Math.Pow(t, 3) + b4 * Math.Pow(t, 4) + b5 * Math.Pow(t, 5)) * 100;
        return comp;
    }
}
