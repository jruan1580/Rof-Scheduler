import { makeHttpRequest } from "./httpClientWrapper";

export const GetAllJobEventsByMonthAndYear = async function(date){
  var baseUrl = process.env.REACT_APP_EVENT_SERVICE_BASE_URL;
  
  var url = baseUrl + "/event?date=" + date;

  return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}

export const addEvent = async function(employeeId, petId, petServiceId, eventStartTime){
  var baseUrl = process.env.REACT_APP_EVENT_SERVICE_BASE_URL + "/event";

  var data = { employeeId, petId, petServiceId, eventStartTime };

  return await makeHttpRequest(baseUrl, "POST", {"Content-Type": "application/json"}, 200, data); 
}