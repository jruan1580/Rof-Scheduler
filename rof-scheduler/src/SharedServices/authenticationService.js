import  { makeHttpRequest } from './httpClientWrapper';

export const login = async function (username, password) {
    var baseUrl = process.env.REACT_APP_AUTH_SERVICE_BASE_URL;
    var data = { username, password };

    var response = await makeHttpRequest(baseUrl + "/authentication/login", "PATCH", {"Content-Type": "application/json"}, 200, data);

    return await response.json();   
  }
  
  export const logoff = async function () {
    var baseUrl = process.env.REACT_APP_AUTH_SERVICE_BASE_URL;
  
    var id = parseInt(localStorage.getItem("id"));    
    var url = baseUrl + "/authentication/" + id + "/logout";

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