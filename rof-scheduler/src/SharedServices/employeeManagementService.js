export const login = async function (username, password) {
  const baseUrl = process.env.REACT_APP_EMPLOYEE_MANAGEMENT_BASE_URL;
  const data = { username, password };

  var response = await fetch(baseUrl + "/employee/login", {
    method: "PATCH",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data), // body data type must match "Content-Type" header
    credentials: "include",
  });

  if (response.status !== 200) {
    var errMsg = await response.text();
    throw new Error(errMsg);
  }

  return await response.json();
};

export const logoff = async function () {
  const baseUrl = process.env.REACT_APP_EMPLOYEE_MANAGEMENT_BASE_URL;

  const id = parseInt(localStorage.getItem("id"));
  const role = localStorage.getItem("role");
  const url =
    role.toLowerCase() === "administrator"
      ? baseUrl + "/admin/logout/" + id
      : baseUrl + "/employee/logout/" + id;

  var response = await fetch(url, {
    method: "PATCH",
    headers: {
      "Content-Type": "application/json",
    },
    credentials: "include",
  });

  if (response.status !== 200) {
    var errMsg = await response.text();
    throw new Error(errMsg);
  }
};

export const getEmployeeById = async function () {
  const baseUrl = process.env.REACT_APP_EMPLOYEE_MANAGEMENT_BASE_URL;

  const id = parseInt(localStorage.getItem("id"));
  const role = localStorage.getItem("role");
  const url =
    role.toLowerCase() === "administrator"
      ? baseUrl + "/admin/" + id
      : baseUrl + "/employee/" + id;

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

export const updateEmployeeInformation = async function (
  id,
  firstName,
  lastName,
  ssn,
  role,
  username,
  addressLine1,
  addressLine2,
  city,
  state,
  zipCode
) {
  const baseUrl = process.env.REACT_APP_EMPLOYEE_MANAGEMENT_BASE_URL;
  const data = {
    id,
    firstName,
    lastName,
    ssn,
    role,
    username,
    address: {
      addressLine1,
      addressLine2,
      city,
      state,
      zipCode,
    },
  };

  role = localStorage.getItem("role");
  const url =
    role.toLowerCase() === "administrator"
      ? baseUrl + "/admin/info"
      : baseUrl + "/employee/info";

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
