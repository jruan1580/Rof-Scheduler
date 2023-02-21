import  { makeHttpRequest } from './httpClientWrapper';

export const getPetTypes = async function(){
    var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
    var url = baseUrl + "/dropdown/petTypes";
  
    return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}

export const getClients = async function(){
    var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
    var url = baseUrl + "/dropdown/clients";
  
    return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}

export const getBreedByPetType = async function(petTypeId){
    var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
    var url = baseUrl + "/dropdown/" + petTypeId + "/breeds";
  
    return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}

export const getVaccinesByPetType = async function(petTypeId){
    var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
    var url = baseUrl + "/dropdown/" + petTypeId + "/vaccines";
  
    return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}

export const getPetServices = async function(){
    var baseUrl = process.env.REACT_APP_PET_SERVICE_BASE_URL;
    var url = baseUrl + "/dropdown/petServices";

    return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}

export const getPets = async function(){
    var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
    var url = baseUrl + "/dropdown/pets";
  
    return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}

export const getEmployees = async function(){
    var baseUrl = process.env.REACT_APP_EMPLOYEE_MANAGEMENT_BASE_URL;
    var url = baseUrl +  "/dropdown/employees";

    return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}