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

  var response = await fetch(baseUrl + "/client", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
    credentials: "include",
  });

  if (response.status !== 201) {
    var errMsg = await response.text();
    throw new Error(errMsg);
  }
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

  var response = await fetch(url, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
    credentials: "include",
  });

  if (response.status !== 200) {
    var errMsg = await response.text();
    throw new Error(errMsg);
  }

  return await response.json();
};

export const resetClientLockStatus = async function (id) {
  var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
  var url = baseUrl + "/client/" + id + "/locked";

  var response = await fetch(url, {
    method: "PATCH",
    credentials: "include",
  });

  if (response.status !== 200) {
    var errMsg = await response.text();
    throw new Error(errMsg);
  }
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

  var response = await fetch(url, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
    credentials: "include",
  });

  if (response.status !== 200) {
    var errMsg = await response.text();
    throw new Error(errMsg);
  }
};
