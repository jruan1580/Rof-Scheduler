export const validateLoginPassword = function (password) {
  var errMsg = "";

  if (password === undefined || password.length == 0) {
    errMsg = "Please enter a password.";
    return errMsg;
  }

  if (password.length < 8 || password.length > 32) {
    errMsg = "Ensure password length is between 8 and 32 characters.";
    return errMsg;
  }

  var lowerCaseLetters = /[a-z]/g;
  var upperCaseLetters = /[A-Z]/g;
  var numbers = /[0-9]/g;
  var specialChars = /[`!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/g;

  if (
    !lowerCaseLetters.test(password) ||
    !upperCaseLetters.test(password) ||
    !numbers.test(password) ||
    !specialChars.test(password)
  ) {
    errMsg =
      "Ensure that password has lower AND upper cases characters, digits, and special characters.";
    return errMsg;
  }

  return errMsg;
};

export const ensureCreateEmployeeInformationProvided = function (
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
  zipCode,
  password,
  retypedPassword
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

  if (email === undefined || email === "") {
    validationErrors.set("email", "Please enter an email address.");
  }

  if (phoneNumber === undefined || phoneNumber === "") {
    validationErrors.set("phone", "Please enter a phone number.");
  }

  var pwErrMsg = validateLoginPassword(password);
  if (pwErrMsg !== "") {
    validationErrors.set("password", pwErrMsg);
  }

  if (retypedPassword === undefined || retypedPassword === "") {
    validationErrors.set("retypedPassword", "Please retype password.");
  } else {
    if (retypedPassword !== password) {
      validationErrors.set("retypedPassword", "Passwords did not match.");
    }
  }

  return validationErrors;
};

export const ensureEmployeeUpdateInformationProvided = function (
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

  if (email === undefined || email === "") {
    validationErrors.set("email", "Please enter an email address.");
  }

  if (phoneNumber === undefined || phoneNumber === "") {
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

export const ensureClientUpdateInformationProvided = function (
  firstName,
  lastName,
  username,
  email,
  primaryPhoneNum,
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

  if (email === undefined || email === "") {
    validationErrors.set("email", "Please enter an email address.");
  }

  if (primaryPhoneNum === undefined || primaryPhoneNum === "") {
    validationErrors.set("phone", "Please enter a phone number.");
  }

  return validationErrors;
};

export const ensureAddPetInformationProvided = function (
  petName,
  breed,
  weight,
  dob,
  client
) {
  var validationErrors = new Map();

  if (petName === undefined || petName === "") {
    validationErrors.set("petName", "Please enter pet's name.");
  }

  if (breed === undefined || breed <= 0) {
    validationErrors.set("breed", "Please select pet's breed.");
  }

  if (weight === undefined || isNaN(weight)) {
    validationErrors.set("weight", "Please enter pet's weight");
  }

  if (dob === undefined || dob === "") {
    validationErrors.set("dob", "Please enter pet's DOB");
  }

  if (
    localStorage.getItem("role").toLowerCase() !== "client" &&
    (client === undefined || client <= 0)
  ) {
    validationErrors.set("client", "Please select pet's owner");
  }

  return validationErrors;
};

export const ensurePetUpdateInformationProvided = function (
  petName,
  weight,
  dob,
  breedId,
  ownerId
) {
  var validationErrors = new Map();

  if (petName === undefined || petName === "") {
    validationErrors.set("petName", "Please enter pet's name.");
  }

  if (weight === undefined || isNaN(weight)) {
    validationErrors.set("weight", "Please enter pet's weight");
  }

  if (dob === undefined || dob === "") {
    validationErrors.set("dob", "Please enter pet's DOB");
  }

  if (breedId === undefined || breedId <= 0) {
    validationErrors.set("breed", "Please select pet's breed.");
  }

  if (ownerId === undefined || ownerId <= 0) {
    validationErrors.set("owner", "Please select pet's owner");
  }

  return validationErrors;
};
