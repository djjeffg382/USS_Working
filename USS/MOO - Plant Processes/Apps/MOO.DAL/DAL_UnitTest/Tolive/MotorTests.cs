using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class MotorTests : BaseTestClass
    {
        [Fact]
        public void TestMotor_Site()
        {
            DAL.Models.Motor_Site ms = DAL.Services.Motor_SiteSvc.Get(1);
            Assert.NotNull(ms);
            ms = new()
            {
                Motor_Site_Name = "Test Motor Site"
            };

            DAL.Services.Motor_SiteSvc.Insert(ms);
            ms.Motor_Site_Name = "Test update";
            int n = DAL.Services.Motor_SiteSvc.Update(ms);
            Assert.Equal(1, n);
            n = DAL.Services.Motor_SiteSvc.Delete(ms);
            Assert.Equal(1, n);

            //delete just marks the delete column, lets delete the record to clean up the datbase
            MOO.Data.ExecuteNonQuery($"DELETE from tolive.motor_site WHERE motor_site_id = {ms.Motor_Site_Id}", MOO.Data.MNODatabase.DMART);

            var mtrList = DAL.Services.Motor_SiteSvc.GetAll(true);
            Assert.NotNull(mtrList);
        }

        [Fact]
        public async Task TestMotor_Area()
        {
            DAL.Models.Motor_Area ma = DAL.Services.Motor_AreaSvc.Get(0);
            Assert.NotNull(ma);

            ma = new()
            {
                Motor_Area_Desc = "Test Motor Area",
                Motor_Site = DAL.Services.Motor_SiteSvc.Get(1)
            };

            await Task.Run(()=>DAL.Services.Motor_AreaSvc.Insert(ma));

            ma.Motor_Area_Desc = "Test Update";
            int n = DAL.Services.Motor_AreaSvc.Update(ma);
            Assert.Equal(1, n);
            n = DAL.Services.Motor_AreaSvc.Delete(ma);
            Assert.Equal(1, n);
            //delete just marks the delete column, lets delete the record to clean up the datbase
            MOO.Data.ExecuteNonQuery($"DELETE from tolive.motor_area WHERE motor_area_id = {ma.Motor_Area_Id}", MOO.Data.MNODatabase.DMART);


            var mtrList = DAL.Services.Motor_AreaSvc.GetAll(true);
            Assert.NotNull(mtrList);
            mtrList = DAL.Services.Motor_AreaSvc.GetAll(false);
            Assert.NotNull(mtrList);
        }


        [Fact]
        public void TestMotor_Manufacturer()
        {
            DAL.Models.Motor_Manufacturer mm = DAL.Services.Motor_ManufacturerSvc.Get(3877);
            Assert.NotNull(mm);

            mm = new()
            {
                Motor_Manufacturer_Desc = "Test Description"
            };

            DAL.Services.Motor_ManufacturerSvc.Insert(mm);

            mm.Motor_Manufacturer_Desc = "Test Update";
            int n = DAL.Services.Motor_ManufacturerSvc.Update(mm);
            Assert.Equal(1, n);
            n = DAL.Services.Motor_ManufacturerSvc.Delete(mm);
            Assert.Equal(1, n);
            //delete just marks the delete column, lets delete the record to clean up the datbase
            MOO.Data.ExecuteNonQuery($"DELETE from tolive.motor_manufacturer WHERE motor_manufacturer_id = {mm.Motor_Manufacturer_Id}", MOO.Data.MNODatabase.DMART);


            var mtrList = DAL.Services.Motor_ManufacturerSvc.GetAll(true);
            Assert.NotNull(mtrList);

            mtrList = DAL.Services.Motor_ManufacturerSvc.GetAll(false);
            Assert.NotNull(mtrList);
        }


        [Fact]
        public void TestMotor_Status()
        {
            DAL.Models.Motor_Status m = DAL.Services.Motor_StatusSvc.Get(3973);
            Assert.NotNull(m);

            m = new()
            {
                Motor_Status_Name = "Test Description"
            };

            DAL.Services.Motor_StatusSvc.Insert(m);

            m.Motor_Status_Name = "Test Update";
            int n = DAL.Services.Motor_StatusSvc.Update(m);
            Assert.Equal(1, n);
            n = DAL.Services.Motor_StatusSvc.Delete(m);
            Assert.Equal(1, n);

            //delete just marks the delete column, lets delete the record to clean up the datbase
            MOO.Data.ExecuteNonQuery($"DELETE from tolive.motor_status WHERE motor_status_id = {m.Motor_Status_Id}", MOO.Data.MNODatabase.DMART);


            var mtrList = DAL.Services.Motor_StatusSvc.GetAll(true);
            Assert.NotNull(mtrList);

            mtrList = DAL.Services.Motor_StatusSvc.GetAll(false);
            Assert.NotNull(mtrList);
        }



        [Fact]
        public void TestMotor_Location()
        {
            DAL.Models.Motor_Location m = DAL.Services.Motor_LocationSvc.Get(9);
            Assert.NotNull(m);

            m = new()
            {
                Motor_Location_Desc = "Test Description",
                Motor_Area = DAL.Services.Motor_AreaSvc.Get(0)
            };

            DAL.Services.Motor_LocationSvc.Insert(m);

            m.Motor_Location_Desc = "Test Update";
            int n = DAL.Services.Motor_LocationSvc.Update(m);
            Assert.Equal(1, n);
            n = DAL.Services.Motor_LocationSvc.Delete(m);
            Assert.Equal(1, n);
            //delete just marks the delete column, lets delete the record to clean up the datbase
            MOO.Data.ExecuteNonQuery($"DELETE from tolive.motor_location WHERE motor_location_id = {m.Motor_Location_Id}", MOO.Data.MNODatabase.DMART);



            MOO.DAL.PagedData<List<DAL.Models.Motor_Location>> mtrList =
                    DAL.Services.Motor_LocationSvc.GetPagedData(false, null, "001", 0, 20);
            Assert.NotNull(mtrList);

            mtrList =
                    DAL.Services.Motor_LocationSvc.GetPagedData(false, 0, "001", 0, 20);
            Assert.NotNull(mtrList);

        }

        [Fact]
        public void TestMotor_Motors()
        {
            var AllMtr = DAL.Services.Motor_MotorsSvc.GetAll(true);
            Assert.NotNull(AllMtr);
            Assert.NotEmpty(AllMtr);
            var pd = DAL.Services.Motor_MotorsSvc.GetPagedData(false, "X1816", null, null, null, 0, 20);
            Assert.NotNull(pd);
            DAL.Models.Motor_Motors m = DAL.Services.Motor_MotorsSvc.Get(4354);
            Assert.NotNull(m);
            Random r = new();

            //because we have so many fields, we will ensure they are in correct order by inserting, and then selecting, and then compare
            m = new()
            {
                Catalog_Id_Number = r.Next(99999),
                Critical_Motor = true,
                Deleted_Record = false,
                Drive_Configuration = DAL.Models.Motor_Motors.Drive_Config.Direct,
                Equip_Serial_Num = r.Next().ToString(),
                Explosion_Proof = true,
                Frame_Size = r.Next(99999).ToString(),
                Full_Load_Amps = r.Next(99999),
                Horse_Power = r.Next(99999),
                Inner_Bearing = r.Next(99999).ToString(),
                Outer_Bearing = r.Next(999999).ToString(),
                Instruction_Book = r.Next(99999).ToString(),
                Insulation_Class = r.Next(99999).ToString(),
                Inverter_Rated = true,
                J_Box_Setup = DAL.Models.Motor_Motors.JBox.F2,
                Last_Modified_By = r.Next(999999).ToString(),
                Last_Modified_Date = DateTime.Now,
                Motor_Location = DAL.Services.Motor_LocationSvc.Get(9),
                Motor_Manufacturer = DAL.Services.Motor_ManufacturerSvc.Get(3877),
                Motor_Status = DAL.Services.Motor_StatusSvc.Get(3973),
                Motor_Type = r.Next(99999).ToString(),
                Notes = r.Next(999999).ToString(),
                Phase = r.Next(999999),
                Purchase_Date = DateTime.Now.AddDays(-1),
                Rpm = r.Next(999999),
                RTD_Type = r.Next(999999).ToString(),
                Service_Factor = r.Next(999999),
                Voltage_Rating = r.Next(999999),
                Weight = r.Next(9999999)

            };


            DAL.Services.Motor_MotorsSvc.Insert(m);
            //now get it back from the database
            DAL.Models.Motor_Motors mCopy = DAL.Services.Motor_MotorsSvc.Get(m.Motor_Equipment_Id);
            Assert.Equal(mCopy.Catalog_Id_Number, m.Catalog_Id_Number);
            Assert.Equal(mCopy.Critical_Motor, m.Critical_Motor);
            Assert.Equal(mCopy.Deleted_Record, m.Deleted_Record);
            Assert.Equal(mCopy.Drive_Configuration, m.Drive_Configuration);
            Assert.Equal(mCopy.EquipType, m.EquipType);
            Assert.Equal(mCopy.Equip_Serial_Num, m.Equip_Serial_Num);
            Assert.Equal(mCopy.Explosion_Proof, m.Explosion_Proof);
            Assert.Equal(mCopy.Frame_Size, m.Frame_Size);
            Assert.Equal(mCopy.Full_Load_Amps, m.Full_Load_Amps);
            Assert.Equal(mCopy.Horse_Power, m.Horse_Power);
            Assert.Equal(mCopy.Inner_Bearing, m.Inner_Bearing);
            Assert.Equal(mCopy.Outer_Bearing, m.Outer_Bearing);
            Assert.Equal(mCopy.Instruction_Book, m.Instruction_Book);
            Assert.Equal(mCopy.Insulation_Class, m.Insulation_Class);
            Assert.Equal(mCopy.Inverter_Rated, m.Inverter_Rated);
            Assert.Equal(mCopy.J_Box_Setup, m.J_Box_Setup);
            Assert.Equal(mCopy.Last_Modified_By, m.Last_Modified_By);
            Assert.Equal(mCopy.Motor_Location.Motor_Location_Id, m.Motor_Location.Motor_Location_Id);
            Assert.Equal(mCopy.Motor_Manufacturer.Motor_Manufacturer_Id, m.Motor_Manufacturer.Motor_Manufacturer_Id);
            Assert.Equal(mCopy.Motor_Status.Motor_Status_Id, m.Motor_Status.Motor_Status_Id);
            Assert.Equal(mCopy.Motor_Type, m.Motor_Type);
            Assert.Equal(mCopy.Notes, m.Notes);
            Assert.Equal(mCopy.Phase, m.Phase);
            Assert.Equal(mCopy.Rpm, m.Rpm);
            Assert.Equal(mCopy.RTD_Type, m.RTD_Type);
            Assert.Equal(mCopy.Service_Factor, m.Service_Factor);
            Assert.Equal(mCopy.Voltage_Rating, m.Voltage_Rating);
            Assert.Equal(mCopy.Weight, m.Weight);

            //now run the update and get the record from the database, nothing should have changed
            DAL.Services.Motor_MotorsSvc.Update(m);
            //now get it back from the database
            mCopy = DAL.Services.Motor_MotorsSvc.Get(m.Motor_Equipment_Id);
            Assert.Equal(mCopy.Catalog_Id_Number, m.Catalog_Id_Number);
            Assert.Equal(mCopy.Critical_Motor, m.Critical_Motor);
            Assert.Equal(mCopy.Deleted_Record, m.Deleted_Record);
            Assert.Equal(mCopy.Drive_Configuration, m.Drive_Configuration);
            Assert.Equal(mCopy.EquipType, m.EquipType);
            Assert.Equal(mCopy.Equip_Serial_Num, m.Equip_Serial_Num);
            Assert.Equal(mCopy.Explosion_Proof, m.Explosion_Proof);
            Assert.Equal(mCopy.Frame_Size, m.Frame_Size);
            Assert.Equal(mCopy.Full_Load_Amps, m.Full_Load_Amps);
            Assert.Equal(mCopy.Horse_Power, m.Horse_Power);
            Assert.Equal(mCopy.Inner_Bearing, m.Inner_Bearing);
            Assert.Equal(mCopy.Outer_Bearing, m.Outer_Bearing);
            Assert.Equal(mCopy.Instruction_Book, m.Instruction_Book);
            Assert.Equal(mCopy.Insulation_Class, m.Insulation_Class);
            Assert.Equal(mCopy.Inverter_Rated, m.Inverter_Rated);
            Assert.Equal(mCopy.J_Box_Setup, m.J_Box_Setup);
            Assert.Equal(mCopy.Last_Modified_By, m.Last_Modified_By);
            Assert.Equal(mCopy.Motor_Location.Motor_Location_Id, m.Motor_Location.Motor_Location_Id);
            Assert.Equal(mCopy.Motor_Manufacturer.Motor_Manufacturer_Id, m.Motor_Manufacturer.Motor_Manufacturer_Id);
            Assert.Equal(mCopy.Motor_Status.Motor_Status_Id, m.Motor_Status.Motor_Status_Id);
            Assert.Equal(mCopy.Motor_Type, m.Motor_Type);
            Assert.Equal(mCopy.Notes, m.Notes);
            Assert.Equal(mCopy.Phase, m.Phase);
            Assert.Equal(mCopy.Rpm, m.Rpm);
            Assert.Equal(mCopy.RTD_Type, m.RTD_Type);
            Assert.Equal(mCopy.Service_Factor, m.Service_Factor);
            Assert.Equal(mCopy.Voltage_Rating, m.Voltage_Rating);
            Assert.Equal(mCopy.Weight, m.Weight);



            //delete just marks the delete column, lets delete the record to clean up the datbase
            MOO.Data.ExecuteNonQuery($"DELETE from tolive.motor_motors WHERE motor_equipment_id = {m.Motor_Equipment_Id}", MOO.Data.MNODatabase.DMART);
            MOO.Data.ExecuteNonQuery($"DELETE from tolive.motor_equipment WHERE motor_equipment_id = {m.Motor_Equipment_Id}", MOO.Data.MNODatabase.DMART);


            //test paged data
            MOO.DAL.PagedData<List<DAL.Models.Motor_Motors>> mtrList =
                    DAL.Services.Motor_MotorsSvc.GetPagedData(false, null, null, null, "", 0, 50);
            Assert.NotNull(mtrList);
            mtrList =
                    DAL.Services.Motor_MotorsSvc.GetPagedData(false, "111-15", null, null, "", 0, 50);
            Assert.NotNull(mtrList);
            mtrList =
                    DAL.Services.Motor_MotorsSvc.GetPagedData(false, "111-15", 1, null, "", 0, 50);
            Assert.NotNull(mtrList);
            mtrList =
                    DAL.Services.Motor_MotorsSvc.GetPagedData(false, "", 1, null, "002", 0, 50);
            Assert.NotNull(mtrList);

        }



        [Fact]
        public void TestMotor_Starters()
        {
            DAL.Models.Motor_Starters m = DAL.Services.Motor_StartersSvc.Get(8606);
            Assert.NotNull(m);
            Random r = new();

            //because we have so many fields, we will ensure they are in correct order by inserting, and then selecting, and then compare
            m = new()
            {
                Last_Modified_By = r.Next(999999).ToString(),
                Last_Modified_Date = DateTime.Now,
                Motor_Location = DAL.Services.Motor_LocationSvc.Get(9),
                Motor_Manufacturer = DAL.Services.Motor_ManufacturerSvc.Get(3877),
                Motor_Status = DAL.Services.Motor_StatusSvc.Get(3973),
                Notes = r.Next(999999).ToString(),
                Current_Rating = r.Next(999),
                Instruction_Book = r.Next(9999).ToString(),
                Starter_Type = DAL.Models.Motor_Starters.StarterType.Center_Line,
                Voltage_Rating = r.Next(999),
                Equip_Serial_Num = r.Next(9999).ToString()

            };


            DAL.Services.Motor_StartersSvc.Insert(m);
            //now get it back from the database
            DAL.Models.Motor_Starters mCopy = DAL.Services.Motor_StartersSvc.Get(m.Motor_Equipment_Id);

            Assert.Equal(mCopy.Deleted_Record, m.Deleted_Record);
            Assert.Equal(mCopy.EquipType, m.EquipType);
            Assert.Equal(mCopy.Equip_Serial_Num, m.Equip_Serial_Num);
            Assert.Equal(mCopy.Instruction_Book, m.Instruction_Book);
            Assert.Equal(mCopy.Last_Modified_By, m.Last_Modified_By);
            Assert.Equal(mCopy.Motor_Location.Motor_Location_Id, m.Motor_Location.Motor_Location_Id);
            Assert.Equal(mCopy.Motor_Manufacturer.Motor_Manufacturer_Id, m.Motor_Manufacturer.Motor_Manufacturer_Id);
            Assert.Equal(mCopy.Motor_Status.Motor_Status_Id, m.Motor_Status.Motor_Status_Id);
            Assert.Equal(mCopy.Notes, m.Notes);
            Assert.Equal(mCopy.Voltage_Rating, m.Voltage_Rating);

            //now run the update and get the record from the database, nothing should have changed
            DAL.Services.Motor_StartersSvc.Update(m);
            //now get it back from the database
            mCopy = DAL.Services.Motor_StartersSvc.Get(m.Motor_Equipment_Id);
            Assert.Equal(mCopy.Deleted_Record, m.Deleted_Record);
            Assert.Equal(mCopy.EquipType, m.EquipType);
            Assert.Equal(mCopy.Equip_Serial_Num, m.Equip_Serial_Num);
            Assert.Equal(mCopy.Instruction_Book, m.Instruction_Book);
            Assert.Equal(mCopy.Last_Modified_By, m.Last_Modified_By);
            Assert.Equal(mCopy.Motor_Location.Motor_Location_Id, m.Motor_Location.Motor_Location_Id);
            Assert.Equal(mCopy.Motor_Manufacturer.Motor_Manufacturer_Id, m.Motor_Manufacturer.Motor_Manufacturer_Id);
            Assert.Equal(mCopy.Motor_Status.Motor_Status_Id, m.Motor_Status.Motor_Status_Id);
            Assert.Equal(mCopy.Notes, m.Notes);
            Assert.Equal(mCopy.Voltage_Rating, m.Voltage_Rating);



            //delete just marks the delete column, lets delete the record to clean up the datbase
            MOO.Data.ExecuteNonQuery($"DELETE from tolive.motor_starters WHERE motor_equipment_id = {m.Motor_Equipment_Id}", MOO.Data.MNODatabase.DMART);
            MOO.Data.ExecuteNonQuery($"DELETE from tolive.motor_equipment WHERE motor_equipment_id = {m.Motor_Equipment_Id}", MOO.Data.MNODatabase.DMART);


            //test paged data
            MOO.DAL.PagedData<List<DAL.Models.Motor_Starters>> mtrList =
                    DAL.Services.Motor_StartersSvc.GetPagedData(false, null, null, null, "", 0, 50);
            Assert.NotNull(mtrList);
            mtrList =
                    DAL.Services.Motor_StartersSvc.GetPagedData(false, "02", null, null, "", 0, 50);
            Assert.NotNull(mtrList);
            mtrList =
                    DAL.Services.Motor_StartersSvc.GetPagedData(false, "02", 1, null, "", 0, 50);
            Assert.NotNull(mtrList);
            mtrList =
                    DAL.Services.Motor_StartersSvc.GetPagedData(false, "", 1, null, "002", 0, 50);
            Assert.NotNull(mtrList);
        }

        [Fact]
        public void TestMotor_Change()
        {
            var mcGrd = DAL.Services.Motor_ChangeSvc.GetReqGrdChk();
            Assert.NotNull(mcGrd);
            DAL.Models.Motor_Change mc;
            mc = DAL.Services.Motor_ChangeSvc.Get(35966);
            Assert.NotNull(mc);

            List<DAL.Models.Motor_Change> mcList;
            mcList = DAL.Services.Motor_ChangeSvc.GetByMotor(4176);
            Assert.NotNull(mcList);


            //test the insert by just setting current mc to a negative id 
            mc.Motor_Change_Id = -1;
            mc.Date_Of_Change = DateTime.Now;
            DAL.Services.Motor_ChangeSvc.Insert(mc);

        }

        [Fact]
        public void TestMotor_Test()
        {
            DAL.Models.Motor_Test mt;
            mt = DAL.Services.Motor_TestSvc.Get(35527);
            Assert.NotNull(mt);

            List<DAL.Models.Motor_Test> mcList;
            mcList = DAL.Services.Motor_TestSvc.GetByMotor(7244);
            Assert.NotNull(mcList);


            //test the insert by just setting current mc to a negative id 
            mt.Motor_Test_Id = -1;
            mt.Date_Tested = DateTime.Now;
            DAL.Services.Motor_TestSvc.Insert(mt);

        }

        [Fact]
        public void TestMotor_Maintenance()
        {
            DAL.Models.Motor_Maintenance mt;
            mt = DAL.Services.Motor_MaintenanceSvc.Get(32513);
            Assert.NotNull(mt);

            List<DAL.Models.Motor_Maintenance> mcList;
            mcList = DAL.Services.Motor_MaintenanceSvc.GetByMotor(8598);
            Assert.NotNull(mcList);


            //test the insert by just setting current mc to a negative id 
            mt.Motor_Maintenance_Id = -1;
            mt.Date_Of_Maint = DateTime.Now;
            DAL.Services.Motor_MaintenanceSvc.Insert(mt);

        }
    }
}
