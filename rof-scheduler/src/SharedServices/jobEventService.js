import { makeHttpRequest } from "./httpClientWrapper";

export const getAllJobEventsByMonthAndYear = async function(month, year){
  var baseUrl = process.env.REACT_APP_EVENT_SERVICE_BASE_URL;
  
  var url = baseUrl + "/event?month=" + month + "&year=" + year;

  return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}

export const getAllJobEvents = async function(){
  var baseUrl = process.env.REACT_APP_EVENT_SERVICE_BASE_URL;
  
  var url = baseUrl + "/event/all";

  return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}

export const addEvent = async function(employeeId, petId, petServiceId, eventStartTime){
  var baseUrl = process.env.REACT_APP_EVENT_SERVICE_BASE_URL + "/event";

  var data = { employeeId, petId, petServiceId, eventStartTime };

  return await makeHttpRequest(baseUrl, "POST", {"Content-Type": "application/json"}, 200, data); 
}

export const updateEvent = async function(eventId, employeeId, petId, petServiceId, eventStartTime, isCompleted){
  var baseUrl = process.env.REACT_APP_EVENT_SERVICE_BASE_URL + "/event";

  var data = { eventId, employeeId, petId, petServiceId, eventStartTime, isCompleted };

  return await makeHttpRequest(baseUrl, "PUT", {"Content-Type": "application/json"}, 200, data);
}

export const deleteEvent = async function(eventId){
  var url = process.env.REACT_APP_EVENT_SERVICE_BASE_URL + "/event/"+ eventId;

  return await makeHttpRequest(url, "DELETE", undefined, 200, undefined); 
}