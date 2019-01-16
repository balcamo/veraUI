package handlers

import (
    "encoding/json"
    "io"
    "io/ioutil"
    "net/http"
	"meters"
    "github.com/gin-gonic/gin"
)

// GetMetersListHandler returns all current todo items
func GetMetersListHandler(c *gin.Context) {
    c.JSON(http.StatusOK, meters.Get())
}

// AddTodoHandler adds a new todo to the todo list
/*func AddTodoHandler(c *gin.Context) {
    meterItem, statusCode, err := convertHTTPBodyToMeters(c.Request.Body)
    if err != nil {
        c.JSON(statusCode, err)
        return
    }
    c.JSON(statusCode, gin.H{"id": todo.Add(todoItem.Message)})
}

// DeleteTodoHandler will delete a specified todo based on user http input
func DeleteTodoHandler(c *gin.Context) {
    todoID := c.Param("id")
    if err := todo.Delete(todoID); err != nil {
        c.JSON(http.StatusInternalServerError, err)
        return
    }
    c.JSON(http.StatusOK, "")
}

// CompleteTodoHandler will complete a specified todo based on user http input
func CompleteTodoHandler(c *gin.Context) {
    todoItem, statusCode, err := convertHTTPBodyToTodo(c.Request.Body)
    if err != nil {
        c.JSON(statusCode, err)
        return
    }
    if todo.Complete(todoItem.ID) != nil {
        c.JSON(http.StatusInternalServerError, err)
        return
    }
    c.JSON(http.StatusOK, "")
}*/

func convertHTTPBodyToMeters(httpBody io.ReadCloser) (meters.Meter, int, error) {
    body, err := ioutil.ReadAll(httpBody)
    if err != nil {
        return meters.Meter{}, http.StatusInternalServerError, err
    }
    defer httpBody.Close()
    return convertJSONBodyToMeters(body)
}

func convertJSONBodyToMeters(jsonBody []byte) (meters.Meter, int, error) {
    var metersItem meters.Meter
    err := json.Unmarshal(jsonBody, &metersItem)
    if err != nil {
        return meters.Meter{}, http.StatusBadRequest, err
    }
    return metersItem, http.StatusOK, nil
}