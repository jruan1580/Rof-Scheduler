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
