export const makeHttpRequest = async function(url, method, headers, expectedStatusCode, data){
    var requestInit = (data === undefined) ? 
        {
            method: method,
            headers: headers,
            credentials: "include"
        }:
        {
            method: method,
            headers: headers,
            body: JSON.stringify(data),
            credentials: "include"
        };

    var response = await fetch(url, requestInit);

     //clear storage if unauth
     if (response.status === 401){
        localStorage.clear();
        return response;
    }

    if (response.status !== expectedStatusCode){
        var errMsg = await response.text();
        throw new Error(errMsg);
    }
        
    return response;
}