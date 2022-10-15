import  { makeHttpRequest } from './httpClientWrapper';

export const getAllHolidays = async function(page, recPerPage, holidayName){
    var baseUrl = process.env.REACT_APP_PET_SERVICE_BASE_URL;
    
    var url = baseUrl + "/holiday?page=" + page + "&offset=" + recPerPage + "&keyword=" + holidayName;
  
    return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
  }