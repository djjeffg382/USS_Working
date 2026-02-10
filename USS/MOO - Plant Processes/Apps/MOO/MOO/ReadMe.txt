Change logs updated to IT Systems table

Version 8.0.10
- Updated SendMail to record recipients to the recipients2 column in Mail_Log table.  This is a CLOB field instead of varchar so we won't cut off when there are large number of recipients

Version 8.0.9
- modified default value for LIMS_DB_Name

Version 8.0.8
- Added settings properties for LIMS (GlobalSettings.json)

Version 8.0.7
- Added function Dates.GetQuarterDates

Version 8.0.6
- Modified verbiage in EmailSubscription to include USS Users and not just Minntac/Keetac users

Version 8.0.5
- Added Data.GetOraTableComments and OraTableColumnComment Model

Version 8.0.4
 - TnsnamesOraParser class
 - Added Data.GetConnectionStringFromTNSNames

Version 8.0.3
- Added GetAttribute and GetDisplay functions to the EnumExtension class

Version 8.0.2
- Fixed Enum.GetDescription error when an enum integer value is used that is not in the enum list.  Returns the value instead

Version 8.0.1
- Fixed error when Calling Data.ReadDBKey and the DB Key value is null.

Version 8.0.0
- Upgraded to .Net 8.0

Version 6.0.33
- Added GetEnumByDescription to MOO.Enums.Extension.EnumExtension class

Version 6.0.32
-  Added MOO.Enums.Extension

Version 6.0.31
- Added Data.ExecuteReader function.  This will give ability to execute reader with retries

Version 6.0.28 
- Modified the Conc schedule to match the crusher

Version 6.0.18 - 6.0.27
- Added ability to get a ShiftVal object from the Shift8 class.  This allows use of NextShift and PreviousShift
- Added new Crusher schedule to the Shift class for 12 hours

Version 6.0.16
- Added MineStar datbabase to MOO.Data

Version 6.0.16
- Added MtcWencoReport and KtcWencoReport to MOO.Data. MtcMineReport and KtcMineReport are now obsolete as we will be using separate SQL Servers for this

Version 6.0.15
- Added ability to add program name and description to MOO.Data.WriteDBKey (optional parameters)

Version 6.0.14
- Changed location of EmailSubscription page to http://www.mno.uss.com/bzr/MOO_General/notifications from http://www.mno.uss.com/MNOWeb/Common/EmailSubscription/EmailSubscription.aspx
- Added MtcMineReport and KtcMineReport to Data.MNODatabase enum

Version 6.0.11
Added Network.GetBroadcastAddress

Version 6.0.10
- Added MOO.Network class with networking functions

Version 6.0.9     
- Modified CheckIfShouldRetryQuery in Data class.  Ora errors needed to have spaces removed for checks

Version 6.0.7
- Added retry for SQL Server deadlock in Data class

Version 6.0.6
- Add PI_Point_Addition to the ErrorLog.ErrorLogType enum

Version 6.0.4
- Added code for shift schedule of Minntac Pit

Version 6.0.3
- Added LIMS_Read and LIMS_Report database connection

Version 6.0.1
- Added NextShift() and PreviousShift to ShiftVal class
- Added overload function GetShiftVal(MOO.Plant Plant, MOO.Area Area, DateTime ShiftDate, byte ShiftNbr)

Version 6.0.0 2/4/2022
- Moved to .Net 6.0
- Added ShiftVal class and Shift.GetShiftVal function

Version 5.0.9
- Added GetNextSequence for SQL Server

Version 5.0.8
- Added OraBSCAdmin connection (needs to be added to connections.json file on each server)

Version 5.0.3
- Added Shift and Shift8 classes
- Added enums for plant and area

Version 5.0.2
- Added MOO.Data.ExecuteQuery(DataAdapter)
- Removed Classes (These are now in MOO.DAL library)
	-Security.Sec_Role
	-Security.Sec_RoleSvc
	-Security.Sec_User
	-Security.Sec_UserSvc

Version 5.0.1 (9/24/2021)
- Added classes
	-Security.Sec_Role
	-Security.Sec_RoleSvc
	-Security.Sec_User
	-Security.Sec_UserSvc
	-General
- Add functions
	- Data.DataTableToObjList
	- Data.DataRowToObj

Version 5.0.0
- Original Version