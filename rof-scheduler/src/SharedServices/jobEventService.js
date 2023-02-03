import { makeHttpRequest } from "./httpClientWrapper";

export const GetAllJobEventsByMonthAndYear = async function(date){
  var baseUrl = process.env.REACT_APP_EVENT_SERVICE_BASE_URL;
  
  var url = baseUrl + "/event?date=" + date;

  return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);
}