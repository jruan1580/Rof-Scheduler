import  { makeHttpRequest } from './httpClientWrapper';

export const getRevenueBetweenDatesByPetService = async function (startDate, endDate) {
  var baseUrl = process.env.REACT_APP_DATAMART_SERVICE_BASE_URL;
  var url = baseUrl + "/revenueSummary?startDate=" + startDate + "&endDate=" + endDate;

  return await makeHttpRequest(url, "GET", { "Accept": "application/json"}, 200, undefined);  
};

export const getPayrollBetweenDatesByEmployee = async function (firstName, lastName, startDate, endDate, page) {
  var baseUrl = process.env.REACT_APP_DATAMART_SERVICE_BASE_URL;
  var url = baseUrl + "/payrollSummary?firstName=" + firstName + "&lastName=" + lastName + "&startDate=" + startDate + "&endDate=" + endDate + "&page=" + page;

  return await makeHttpRequest(url, "GET", {"Accept": "application/json"}, 200, undefined);  
};