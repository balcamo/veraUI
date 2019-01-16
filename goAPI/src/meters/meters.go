package meters

/*import (
	"sync"
)

var(
	list []Meter
	mtx sync.RWMutex
    once sync.Once
)

func init() {
    once.Do(initialiseList)
}

func initialiseList() {
    list = []Meter{}
	reads1 := []MeterRead{}
	reads2 := []MeterRead{}
	reads3 := []MeterRead{}
	z := MeterRead{Date:"01/09/2019",Kwh:"23"}
	y := MeterRead{Date:"01/10/2019",Kwh:"30"}
	x := MeterRead{Date:"01/11/2019",Kwh:"20"}
	w := MeterRead{Date:"01/09/2019",Kwh:"32"}
	u := MeterRead{Date:"01/10/2019",Kwh:"28"}
	t := MeterRead{Date:"01/11/2019",Kwh:"25"}
	s := MeterRead{Date:"01/09/2019",Kwh:"15"}
	r := MeterRead{Date:"01/10/2019",Kwh:"22"}
	q := MeterRead{Date:"01/11/2019",Kwh:"17"}
	reads1 = append(reads1,z)	
	reads1 = append(reads1,y)	
	reads1 = append(reads1,x)
	reads2 = append(reads1,w)	
	reads2 = append(reads1,u)	
	reads2 = append(reads1,t)
	reads3 = append(reads1,s)	
	reads3 = append(reads1,r)	
	reads3 = append(reads1,q)
	a := Meter{
		MeterNumber:"001",
		Endpoint:"001",
		Reads:reads1,
		Substation:"6",
		Freq:"11.0023",
		MaxkW:"80",
		Phase:"A"}
	b := Meter{
		MeterNumber:"002",
		Endpoint:"002",
		Reads:reads2,
		Substation:"6",
		Freq:"11.4239",
		MaxkW:"80",
		Phase:"B"}	
	c := Meter{
		MeterNumber:"003",
		Endpoint:"003",
		Reads:reads3,
		Substation:"6",
		Freq:"10.0423",
		MaxkW:"80",
		Phase:"C"}

	list = append(list, a)
	list = append(list, b)
	list = append(list, c)
}

// Meter structure
type Meter struct{
	MeterNumber	string `json:"MeterNumber"`
	Endpoint	string `json:"Endpoint"`
	Reads		[]MeterRead
	Substation	string `json:"Substation"`
	Freq		string `json:"Freq"`
	MaxkW		string `json:"MaxkW"`
	Phase		string `json:"Phase"`
}

type MeterRead struct{
	Date	string `json:"Date"`
	Kwh		string `json:"Kwh"`
}

// retrieve the list of meters
func Get() []Meter{
	return list
}

func isMatchingMeterNumber(a string, b string) bool {
    return a == b
}*/

import (
    _ "github.com/denisenkom/go-mssqldb"
    "database/sql"
    "context"
    "log"
    "fmt"
    "errors"
)

var db *sql.DB

var server = "apiarywebsql-dev.database.windows.net"
var port = 1433
var user = "balcamo"
var password = "message@6whiskey"
var database = "apiaryweb-dev"

func meters(){
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

	count, err := ReadMeters()
    if err != nil {
        log.Fatal("Error reading Employees: ", err.Error())
    }
    fmt.Printf("Read %d row(s) successfully.\n", count)

}

// ReadMeters reads all employee records
func ReadMeters() (int, error) {
    ctx := context.Background()

    // Check if database is alive.
    err := db.PingContext(ctx)
    if err != nil {
        return -1, err
    }

    tsql := fmt.Sprintf("SELECT Id, Name, Location FROM TestSchema.Employees;")

    // Execute query
    rows, err := db.QueryContext(ctx, tsql)
    if err != nil {
        return -1, err
    }

    defer rows.Close()

    var count int

    // Iterate through the result set.
    for rows.Next() {
        var name, location string
        var id int

        // Get values from row.
        err := rows.Scan(&id, &name, &location)
        if err != nil {
            return -1, err
        }

        fmt.Printf("ID: %d, Name: %s, Location: %s\n", id, name, location)
        count++
    }

    return count, nil
}