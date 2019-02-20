 package main

import (
	"handlers"
	//"meters"
	"path"
	"path/filepath"
	"github.com/gin-gonic/gin"
	"github.com/gin-contrib/cors"
)

func main() {
    r := gin.Default()
	
	// CORS for https://foo.com and https://github.com origins, allowing:
    // - PUT and PATCH methods
    // - Origin header
    // - Credentials share
    // - Preflight requests cached for 12 hours
    r.Use(cors.New(cors.Config{
        AllowOrigins:     []string{"https://angularapiary.azurewebsites.net"},
        AllowMethods:     []string{"PUT", "PATCH","GET","POST","DELETE"},
        AllowHeaders:     []string{"*"},
        ExposeHeaders:    []string{"Content-Length"},
        AllowCredentials: true,
        AllowOriginFunc: func(origin string) bool {
            return origin == "https://angularapiary.azurewebsites.net"
        },
    }))

    r.NoRoute(func(c *gin.Context) {
        dir, file := path.Split(c.Request.RequestURI)
        ext := filepath.Ext(file)
        if file == "" || ext == "" {
            c.File("https://angularapiary.azurewebsites.net")
        } else {
            c.File("https://angularapiary.azurewebsites.net/meter-reads" + path.Join(dir, file))
        }
    })

    r.GET("/meters", handlers.GetMetersListHandler)
   
    //r.POST("/todo", handlers.AddTodoHandler)
    //r.DELETE("/todo/:id", handlers.DeleteTodoHandler)
    //r.PUT("/todo", handlers.CompleteTodoHandler)

    err := r.Run(":3000")
    if err != nil {
        panic(err)
    }
}

