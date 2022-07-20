export const get = async function(url) {
    var response = await fetch(url, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
    });
    
    if (response.status !== 200) {
        if (response.status == 401){
            console.log('unauthorized');
            return null;
        }

        var errMsg = await response.text();
        throw new Error(errMsg);
    }

    return await response.json();
}