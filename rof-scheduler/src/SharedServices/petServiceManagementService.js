import  { makeHttpRequest } from './httpClientWrapper';

export const getAllPetServices = async function(page, recPerPage, keyword){
    var baseUrl = process.env.REACT_APP_PET_SERVICE_BASE_URL;
    
    var url = baseUrl + "/petservice?page=" + page + "&offset=" + recPerPage + "&keyword=" + keyword;
  
    return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}

export const addNewPetService = async function(name, rate, employeeRate, description){
    var baseUrl = process.env.REACT_APP_PET_SERVICE_BASE_URL + "/petservice";

    var data = { name, rate, employeeRate, description };

    return await makeHttpRequest(baseUrl, "POST", {"Content-Type": "application/json"}, 200, data); 
}

export const updatePetService = async function(id, name, rate, employeeRate, description){
    var baseUrl = process.env.REACT_APP_PET_SERVICE_BASE_URL + "/petservice";

    var data = { id, name, rate, employeeRate, description };

    return await makeHttpRequest(baseUrl, "PUT", {"Content-Type": "application/json"}, 200, data); 
}