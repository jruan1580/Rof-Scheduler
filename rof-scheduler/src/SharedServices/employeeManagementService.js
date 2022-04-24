export const login = async function(username, password){
    const baseUrl = process.env.REACT_APP_EMPLOYEE_MANAGEMENT_BASE_URL;
    const data = { username, password};

    var response = await fetch(baseUrl + '/employee/login',{
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data), // body data type must match "Content-Type" header
        credentials: 'include'
    });

    if (response.status !== 200){
        var errMsg = await response.text();
        throw new Error(errMsg);
    }

    return await response.json();
}