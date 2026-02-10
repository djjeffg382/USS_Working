Explanation of the Analytics table.  This is similar to the ConcAnalytics table however I feel the naming of Group instead of Pivot makes more sense.  
Perhaps someday, the conc_analytics can be moved to use the Analytics table instead.

The Analytics object is a one for one of the TOLIVE.Analytics table
The AnalyticsGropued provides a list of AnalyticsGrpRec objects with a dates array of all dates available for the records

The AnalyticsGrpRec takes the analytics table and groups it down by Plant, Area, Data_Group, Line_Nbr, Label and the provides the data per date in a list with (date, value, and quality)