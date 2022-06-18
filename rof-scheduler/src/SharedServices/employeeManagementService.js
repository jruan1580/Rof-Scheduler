export const getAllEmployees = async function(page, recPerPage, keyword){
  var baseUrl = process.env.REACT_APP_EMPLOYEE_MANAGEMENT_BASE_URL;
  var url = baseUrl + "/admin?page=" + page + "&offset=" + recPerPage + "&keyword=" + keyword;

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
}

export const getEmployeeById = async function () {
  var baseUrl = process.env.REACT_APP_EMPLOYEE_MANAGEMENT_BASE_URL;

  var id = parseInt(localStorage.getItem("id"));
  var role = localStorage.getItem("role");
  var url =
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

export const createEmployee = async function(
  firstName,
  lastName,
  ssn,
  role,
  username,
  email,
  phoneNumber,
  addressLine1,
  addressLine2,
  city,
  state,
  zipCode,
  password
){
  var baseUrl = process.env.REACT_APP_EMPLOYEE_MANAGEMENT_BASE_URL;
  var data = {
    firstName,
    lastName,
    password,
    ssn,
    role,
    username,
    emailAddress: email,
    phoneNumber,
    address: {
      addressLine1,
      addressLine2,
      city,
      state,
      zipCode
    }
  };

  var url = baseUrl + "/admin";
  
  var response = await fetch(url, {
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
}

export const updateEmployeeInformation = async function (
  id,
  firstName,
  lastName,
  ssn,
  role,
  username,
  email,
  phoneNumber,
  addressLine1,
  addressLine2,
  city,
  state,
  zipCode
) {
  var baseUrl = process.env.REACT_APP_EMPLOYEE_MANAGEMENT_BASE_URL;
  var data = {
    id,
    firstName,
    lastName,
    ssn,
    role,
    username,
    emailAddress: email,
    phoneNumber,
    address: {
      addressLine1,
      addressLine2,
      city,
      state,
      zipCode
    }
  };

  role = localStorage.getItem("role");
  var url =
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

export const resetEmployeeLockStatus = async function(id){
  var baseUrl = process.env.REACT_APP_EMPLOYEE_MANAGEMENT_BASE_URL;
  var url = baseUrl + "/admin/" + id + "/locked";
  
  var response = await fetch(url, {
    method: "PATCH",  
    credentials: "include",
  });

  if (response.status !== 200) {
    var errMsg = await response.text();
    throw new Error(errMsg);
  }
}

export const updateEmployeeStatus = async function(id, status){
  var baseUrl = process.env.REACT_APP_EMPLOYEE_MANAGEMENT_BASE_URL;
  var url = baseUrl + "/admin/" + id;
  url += (status) ? "/activate" : "/deactivate" ;
  
  var response = await fetch(url, {
    method: "PATCH",  
    credentials: "include",
  });

  if (response.status !== 200) {
    var errMsg = await response.text();
    throw new Error(errMsg);
  }
}