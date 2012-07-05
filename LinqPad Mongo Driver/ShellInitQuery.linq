void Main()
{
    
}

//Add additional properties and methods here.  Public members will be visible to queries
//via the CustomInitializer object, e.x. in a query you could write 
//this.CustomInitializer.MyCustomProperty = foo;

/// <summary>
/// Initializes the dynamic driver context.  Called by the driver before any queries are run.
/// </summary>
/// <param name="connectionProperties">The Metadata that was used to build the driver context.</param>
public void Initialize(ConnectionProperties connectionProperties)
{
	/*
	* Initialize the mongo driver context here.  This method will be executed before your
	* query is run, before even the query's properties initialized.  This is the place
	* to register serializer conventions or class maps.

	* Note: LINQPad will re-use AppDomains for queries, but will create new instances
	* of the driver context.  If you want code to run once per AppDomain (ex. registering conventions),
	* create a static boolean and check it.
	*/

}