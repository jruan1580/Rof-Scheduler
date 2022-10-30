import  { makeHttpRequest } from './httpClientWrapper';

export const getAllHolidays = async function(page, recPerPage, holidayName){
  var baseUrl = process.env.REACT_APP_PET_SERVICE_BASE_URL;
  
  var url = baseUrl + "/holiday?page=" + page + "&offset=" + recPerPage + "&keyword=" + holidayName;

  return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}

export const addHoliday = async function(name, month, day){
  var baseUrl = process.env.REACT_APP_PET_SERVICE_BASE_URL + "/holiday";

  var data = { name, month, day };

  return await makeHttpRequest(baseUrl, "POST", {"Content-Type": "application/json"}, 200, data); 
}

export const updateHoliday = async function(id, name, month, day){
  var baseUrl = process.env.REACT_APP_PET_SERVICE_BASE_URL + "/holiday";

  var data = { id, name, month, day };

  return await makeHttpRequest(baseUrl, "PUT", {"Content-Type": "application/json"}, 200, data); 
}