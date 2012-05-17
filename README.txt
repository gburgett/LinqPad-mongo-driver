LinqPad mongo driver v 0.9
Author: Gordon Burgett

Deploying the LINQPad Mongo driver:
	1) Open LinqPad
	2) Select Add Connection
	3) click "View more drivers..."
	4) click "Browse..."
	5) locate LinqpadMongoDriver.lpx and select OK
	6) LinqPad should respond with "Driver successfully installed"
	7) Return to the "Choose Data Context" menu and select "Mongo BSON Driver" by Gordon Burgett, hit Next
	
	At this point we need to configure the driver.  It will at a minimum need a connection string.  If you want to take advantage of the BSON serialization
	and work with strongly-typed objects you will need access to the TripLink IO assemblies.  If someone else has set up their connection before you can ask
	them to export their connection settings using the "Export" button, this will generate an XML file that you can import using the "Import" button.  If this works
	then great! you're good to go.  Otherwise you will need to set it up manually:
	
	8) Enter the connection string and click Connect, this will populate the Collections window and the Database drop-down
	9) Select the Database you wish to use from the database drop down.
	10) Click the "Add" button next to "Assemblies" and locate TripLinkIO.dll, once this is loaded the types window will be populated with the exported types
	10a) If you have imported settings but some assemblies could not be located, double click on the assembly in the "Assemblies" text box to locate it.
	
	11) You must now map collections to types so that the driver will know how to serialize the collections.
		1) Click on the CollectionType box next to the collection you want to map
		2) Find and double-click the type of that collection.  Example: for "settings" you want to find "GDSX.TripLink.IO.Settings.SettingBase"
		or 2) Any collection can be mapped to "MongoDB.Bson.BsonDocument", all the information will be there but it is difficult to work with in queries.
		
	12)	Add Custom Serializers: There may be some custom serializers for certain types that the driver needs to know about.
		1) Click on "Add" next to "Custom Serializers"
		2) Using the left window, select a Type that you want to map to a serializer
		3) Using the right window, select a custom serializer type.
			
	13) Select "Save" to save your settings and populate the connection
	
Using the LINQPad Mongo driver:
	Once configured, the typed collections will show up as tables.  Using this connection you have access to any of the collections you have already mapped to types,
	as instances of IQueryable.  This means you can use Linq queries on the collection objects, for example:
		(from item in items
			where item.fooInt > 2
			select new { item.fooString, item.barObj })
			
	This will translate into a Query that is executed on the mongo collection "items".  You could perform the exact same query on the MongoCollection instance like this:
		itemsCollection.FindAs<Item>(Query.GT("fooInt", "2")).SetFields(Fields.Include("fooString", "barObj")).Select(x => new { x.fooString, foo.barObj });
		
	As you can see, the collections are pluralized and exposed as IQueryable<T> properties on the object in which your query executes.  There is also
	a property for each collection which is a MongoCollection<T> instance.  You also have access to the MongoDatabase object.  
	The exposed properties for the collection "items" which is mapped to the type "Item" look like this:
	
		public String ConnectionString {get;}
		public MongoDatabase db {get;}		
	
		//for each collection
		public IQueryable<Item> items {get;}
		public MongoCollection<Item> itemsCollection {get;}
		
	So you can pretty much do whatever you want with the mongo database.
	
	There is a property on the object called TrackChanges.  If this property is set to true, every object that is returned by Mongo will be serialized again
	and the serialized data stored in memory.  If a change is made to the object, the change can be saved to the database by calling SubmitChanges();.
	Turning on TrackChanges and saving using SubmitChanges uses twice as much serialization as normal, because the objects are serialized and compared to
	their original version when they were read to see if they were intended to be changed.
	Alternately you can just call the .Save<T>(T obj) method on the appropriate collection.

Hints:
	The "IQueryable<T>" versions of the collections do not support all types of queries.  I would suggest having an extension method set up in "My Extensions"
	that looks like this:
	
		public static IEnumerable<T> EndQuery<T>(this IQueryable<T> queryable)
		{
			return queryable;
		}
		
	That way you can call .EndQuery() to tell the Mongo QueryProvider that all the rest of the chain should operate on the enumeration of results from the query
	and not to attempt to translate them into the query itself.
	