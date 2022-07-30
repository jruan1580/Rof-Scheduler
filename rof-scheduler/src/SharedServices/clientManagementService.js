import  { makeHttpRequest } from './httpClientWrapper';

export const createClient = async function (
  countryId,
  firstName,
  lastName,
  emailAddress,
  username,
  password,
  primaryPhoneNum,
  secondaryPhoneNum,
  addressLine1,
  addressLine2,
  city,
  state,
  zipCode
) {
  var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
  var data = {
    countryId,
    firstName,
    lastName,
    emailAddress,
    username,
    password,
    primaryPhoneNum,
    secondaryPhoneNum,
    address: {
      addressLine1,
      addressLine2,
      city,
      state,
      zipCode,
    },
  };

  return await makeHttpRequest(baseUrl + "/client", "POST", {"Content-Type": "application/json"}, 201, data);
};

export const getAllClients = async function (page, recPerPage, keyword) {
  var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
  var url =
    baseUrl +
    "/client?page=" +
    page +
    "&offset=" +
    recPerPage +
    "&keyword=" +
    keyword;

  return await makeHttpRequest(url, "GET", { "Accept": "application/json"}, 200, undefined);  
};

export const getClientById = async function(){
  var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;

  var id = parseInt(localStorage.getItem("id"));
  var url = baseUrl + "/client/" + id;

  return await makeHttpRequest(url, "GET", { "Accept": "application/json"}, 200, undefined);  
}

export const resetClientLockStatus = async function (id) {
  var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
  var url = baseUrl + "/client/" + id + "/locked";

  return await makeHttpRequest(url, "PATCH", { "Accept": "application/json"}, 200, undefined);
};

export const updateClientInformation = async function (
  id,
  firstName,
  lastName,
  username,
  email,
  primaryPhoneNum,
  secondaryPhoneNum,
  addressLine1,
  addressLine2,
  city,
  state,
  zipCode
) {
  var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
  var data = {
    id,
    firstName,
    lastName,
    username,
    emailAddress: email,
    primaryPhoneNum,
    secondaryPhoneNum,
    address: {
      addressLine1,
      addressLine2,
      city,
      state,
      zipCode,
    },
  };

  var url = baseUrl + "/client/info";

  return await makeHttpRequest(url, "PUT", {"Content-Type": "application/json"}, 200, data);  
};
