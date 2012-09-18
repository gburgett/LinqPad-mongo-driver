//Add additional properties and methods here.  Public members will be visible to queries
//via the CustomInitializer object, e.x. in a query you could write 
//this.CustomInitializer.MyCustomProperty = foo;


//this should be reinitialized per app domain
private static bool mInitializedAlready = false;


/// <summary>
/// Initializes the dynamic driver context.  Called by the driver before any queries are run.
/// </summary>
/// <param name="connectionProperties">The Metadata that was used to build the driver context.</param>
public void Initialize(ConnectionProperties connectionProperties)
{
	/*
	* Initialize the mongo driver context here.  This method will be executed before your
	* query is run, before even the query's properties initialized.  Code here will be executed
	* before every query.
	*/

	if(!mInitializedAlready)
	{
		/* LINQPad will re-use AppDomains for queries, but will create new instances
		* of the driver context.  If you want code to run once per AppDomain (ex. registering conventions),
		* put it in here.  This is the place to register serializer conventions or class maps.
		*/

		mInitializedAlready = true;
	}
}	

void Main()
{
    // Test your initialize method here.  This will not be run when the context is initialized.
}