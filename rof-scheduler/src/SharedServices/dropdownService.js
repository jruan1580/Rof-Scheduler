import  { makeHttpRequest } from './httpClientWrapper';

export const getPetTypes = async function(){
    var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
    var url = baseUrl + "/dropdown/petType";
  
    return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}

export const getVaccinesByPetType = async function(petTypeId){
    var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
    var url = baseUrl + "/dropdown/" + petTypeId + "/vaccine";
  
    return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}