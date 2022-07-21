export const get = async function(url) {
    var response = await fetch(url, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
    });

    //clear storage if unauth
    if (response.status === 401){
        localStorage.clear();    
    }

    if (response.status !== 200 && response.status !== 401){
        var errMsg = await response.text();
        throw new Error(errMsg);
    }
        
    return response;
}