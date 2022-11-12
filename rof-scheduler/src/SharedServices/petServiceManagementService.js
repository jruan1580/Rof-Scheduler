import  { makeHttpRequest } from './httpClientWrapper';

export const getAllPetServices = async function(page, recPerPage, keyword){
    var baseUrl = process.env.REACT_APP_PET_SERVICE_BASE_URL;
    
    var url = baseUrl + "/petservice?page=" + page + "&offset=" + recPerPage + "&keyword=" + keyword;
  
    return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}