import { makeHttpRequest } from "./httpClientWrapper"; 
 
export const addEvent = async function(employeeId, petId, petServiceId, eventStartTime){ 
  var baseUrl = process.env.REACT_APP_EVENT_SERVICE_BASE_URL + "/event"; 
 
  const data = { employeeId, petId, petServiceId, eventStartTime }; 
 
  return await makeHttpRequest(baseUrl, "POST", {"Content-Type": "application/json"}, 200, data);  
}