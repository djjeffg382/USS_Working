Items in this folder will be used for the sending to the ERP System.  This will be used by both the Job SendERP and the web version to view the data

The idea behind the services is these will get the data.  There will be to functions for everything.  One will get just the data, this will allow
the ability to show the data on the web or any other troubleshooting.  The second function will be to get the ERPMessage.  This will be used for sending
to ERP.  An example of this is the following:

MinntacAggSvc.GetAggProdLineDayValues - Gets the raw values
MinntacAggSvc.GetAggProdLineDayMessages - Converts the raw values to ERP Messages that can be sent to ERP