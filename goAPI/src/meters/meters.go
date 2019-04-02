package meters



import (
    _ "github.com/denisenkom/go-mssqldb"
    "database/sql"
    "context"
    "log"
    "fmt"
)

// Meter structure
type Meter struct{
	MeterNumber	string `json:"MeterNumber"`
	Manufactuer	string `json:"Manufactuer"`
	ModelNum	string `json:"ModelNum"`
	SerialNum	string `json:"SerialNum"`
	Substation	string `json:"Substation"`
	Feeder		string `json:"Feeder"`
	Address		string `json:"Address"`
	Location	string `json:"Location"`
	BillingType	string `json:"BillingType"`
	BillingCycle string `json:"BillingCycle"`
	Multiplier	string `json:"Multiplier"`
	Route		string `json:"Route"`
	LogDate		string `json:"LogDate"`
	Endpoint	string `json:"Endpoint"`}



var (
	list []Meter
	db *sql.DB
	server = "apiarywebsql-dev.database.windows.net"
	port = 1433
	user = "balcamo"
	password = "message@6whiskey"
	database = "apiaryweb-dev"
)
/*var db *sql.DB

var server = "apiarywebsql-dev.database.windows.net"
var port = 1433
var user = "balcamo"
var password = ""
var database = "apiaryweb-dev"*/

func Meters() ([]Meter){
	// Build connection string
	
	connString := fmt.Sprintf("server=%s;user id=%s;password=%s;port=%d;database=%s;",
		server, user, password, port, database)

	var err error

	// Create connection pool
    db, err = sql.Open("sqlserver", connString)
    if err != nil {
        log.Fatal("Error creating connection pool: ", err.Error())
    }
    ctx := context.Background()
    err = db.PingContext(ctx)
    if err != nil {
        log.Fatal(err.Error())
    }
    fmt.Printf("Connected!\n")

	list, err := ReadMeters()
    if err != nil {
        log.Fatal("Error reading Employees: ", err.Error())
    }
	return list
}

// ReadMeters reads all employee records
func ReadMeters() ([]Meter, error) {
	lst := []Meter{}
    ctx := context.Background()

    // Check if database is alive.
    err := db.PingContext(ctx)
    if err != nil {
        return lst, err
    }
	tsql := fmt.Sprintf("SELECT * FROM dbo.electric_meter;")

    // Execute query
    rows, err := db.QueryContext(ctx, tsql)
    if err != nil {
        return lst, err
    }

    defer rows.Close()



    // Iterate through the result set.
    for rows.Next() {
        
		var meterNumber, manufact, modelNum, serial,substation, feeder string
		var id,address, location,btype,bcycle, multiplier,route,log,endpoint string
		var meter Meter
        // Get values from row.
	
        err := rows.Scan(&id,&meterNumber,&manufact,&modelNum,&serial, &substation,&feeder,&address,&location,&btype,&bcycle, &multiplier,&route,&log,&endpoint)
        if err != nil {
	    //if &id == nil {id = "N/A"}
	    //if &meterNumber == nil {meterNumber = "N/A"}
	    //if &manufact == nil {manufact = "N/A"}
	    //if &modelNum == nil {modelNum = "N/A"}
	    //if &serial == nil {serial = "N/A"}
	    //if &substation == nil {substation = "N/A"}
	    if &feeder == nil {feeder = "N/A"}
	    if &address == nil {address = "N/A"}
	    //if &location == nil {location = "N/A"}
	    //if &btype == nil {btype = "N/A"}
	    //if &bcycle == nil {bcycle = "N/A"}
	    if &multiplier == nil {multiplier = "N/A"}
	    //if &route == nil {route = "N/A"}
	    //if &log == nil {log = "N/A"}
	    //if &endpoint == nil {endpoint = "N/A"}
           // return lst, err
        }

		meter = Meter{
			MeterNumber:meterNumber,
			Manufactuer:manufact,
			ModelNum:modelNum,
			SerialNum:serial,
			Substation:substation,
			Feeder:feeder,
			Address:address,
			Location:location,
			BillingType:btype,
			BillingCycle:bcycle,
			Multiplier:multiplier,
			Route:route,
			LogDate:log,
			Endpoint:endpoint}
		lst = append(lst, meter)
    }

    return lst, nil
}