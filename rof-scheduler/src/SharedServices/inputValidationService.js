export const validateLoginPassword = function (password) {
  var errMsg = "";

  if (password.length == 0) {
    errMsg = "Please enter a password.";
    return errMsg;
  }

  if (password.length < 8 || password.length > 32) {
    errMsg = "Ensure password length is between 8 and 32 characters.";
  }

  return errMsg;
};

export const ensureUpdateInformationProvided = function (
  firstName,
  lastName,
  ssn,
  role,
  username,
  email,
  phoneNumber,
  addressLine1,
  city,
  state,
  zipCode
) {
  var validationErrors = new Map();

  if (firstName === undefined || firstName === "") {
    validationErrors.set("firstName", "Please enter your first name.");
  }

  if (lastName === undefined || lastName === "") {
    validationErrors.set("lastName", "Please enter your last name.");
  }

  if (ssn === undefined || ssn === "") {
    validationErrors.set("ssn", "Please enter your ssn.");
  }

  if (role === undefined || role === "") {
    validationErrors.set("role", "Please select a role.");
  }

  if (username === undefined || username === "") {
    validationErrors.set("username", "Please enter your username.");
  }

  if (addressLine1 === undefined || addressLine1 === "") {
    validationErrors.set("addressLine1", "Please enter your address line 1.");
  }

  if (city === undefined || city === "") {
    validationErrors.set("city", "Please enter your city.");
  }

  if (state === undefined || state === "") {
    validationErrors.set("state", "Please enter your state.");
  }

  if (zipCode === undefined || zipCode === "") {
    validationErrors.set("zipCode", "Please enter your zipcode.");
  }

  if (email === undefined || email === ""){
    validationErrors.set("email", "Please enter an email address.");
  }

  if (phoneNumber === undefined || phoneNumber === ""){
    validationErrors.set("phone", "Please enter a phone number.");
  }

  return validationErrors;
};

export const ensureCreateClientInformationProvided = function (
  firstName,
  lastName,
  email,
  username,
  password,
  primaryPhone,
  addressLine1,
  city,
  state,
  zipCode
) {
  var validationErrors = new Map();

  if (firstName === undefined || firstName === "") {
    validationErrors.set("firstName", "Please enter your first name.");
  }

  if (lastName === undefined || lastName === "") {
    validationErrors.set("lastName", "Please enter your last name.");
  }

  if (email === undefined || email === "") {
    validationErrors.set("email", "Please enter your email.");
  }

  if (username === undefined || username === "") {
    validationErrors.set("username", "Please enter your username.");
  }

  if (password === undefined || password === "") {
    validationErrors.set("password", "Please enter your password.");
  }

  if (primaryPhone === undefined || primaryPhone === "") {
    validationErrors.set("primaryPhone", "Please enter a primary phone.");
  }

  if (addressLine1 === undefined || addressLine1 === "") {
    validationErrors.set("addressLine1", "Please enter your address line 1.");
  }

  if (city === undefined || city === "") {
    validationErrors.set("city", "Please enter your city.");
  }

  if (state === undefined || state === "") {
    validationErrors.set("state", "Please enter your state.");
  }

  if (zipCode === undefined || zipCode === "") {
    validationErrors.set("zipCode", "Please enter your zipcode.");
  }

  return validationErrors;
};
