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
    email,
    phoneNumber,
    address: {
      addressLine1,
      addressLine2,
      city,
      state,
      zipCode,
    },
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
