export const login = async function (username, password) {
    const baseUrl = process.env.REACT_APP_AUTH_SERVICE_BASE_URL;
    const data = { username, password };
  
    var response = await fetch(baseUrl + "/authentication/login", {
      method: "PATCH",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data), // body data type must match "Content-Type" header
      credentials: "include",
    });
  
    if (response.status !== 200){
        var errMsg = await response.text();
        throw new Error(errMsg);
    }
  
    return await response.json();
  }
  
  export const logoff = async function () {
    const baseUrl = process.env.REACT_APP_AUTH_SERVICE_BASE_URL;
  
    const id = parseInt(localStorage.getItem("id"));    
    const url = baseUrl + "/authentication/" + id + "/logout";

    var response = await fetch(url, {
      method: "PATCH",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
    });
  
    if (response.status !== 200) {
      var errMsg = await response.text();
      throw new Error(errMsg);
    }
  };