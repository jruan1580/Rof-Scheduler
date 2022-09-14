import { makeHttpRequest } from "./httpClientWrapper";

export const addPet = async function (
  ownerId,
  petTypeId,
  breedId,
  petName,
  petDob,
  petWeight,
  petOtherInfo,
  vaccines
) {
  var url = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL + "/pet";
  const data = {
    ownerId,
    breedId,
    petTypeId,
    name: petName,
    dob: petDob,
    weight: petWeight,
    otherInfo: petOtherInfo,
    vaccines,
  };

  return await makeHttpRequest(
    url,
    "POST",
    { "Content-Type": "application/json" },
    200,
    data
  );
};

export const getAllPets = async function (page, recPerPage, keyword) {
  var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
  var url =
    baseUrl +
    "/pet?page=" +
    page +
    "&offset=" +
    recPerPage +
    "&keyword=" +
    keyword;

  return await makeHttpRequest(
    url,
    "GET",
    { Accept: "application/json" },
    200,
    undefined
  );
};

export const getPetById = async function (id) {
  var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
  var url = baseUrl + "/pet/" + id;

  return await makeHttpRequest(
    url,
    "GET",
    { Accept: "application/json" },
    200,
    undefined
  );
};

export const getPetsByClientId = async function (page, recPerPage, keyword) {
  var clientId = parseInt(localStorage.getItem("id"));

  var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
  var url =
    baseUrl +
    "/pet/clientId?clientId=" +
    clientId +
    "&page=" +
    page +
    "&offset=" +
    recPerPage +
    "&keyword=" +
    keyword;

  return await makeHttpRequest(
    url,
    "GET",
    { Accept: "application/json" },
    200,
    undefined
  );
};

export const updatePetInformation = async function (
  id,
  petName,
  weight,
  dob,
  ownerId,
  breedId,
  otherInfo,
  vaccines
) {
  var baseUrl = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL;
  var data = {
    id,
    name: petName,
    weight,
    dob,
    ownerId,
    breedId,
    otherInfo,
    vaccines,
  };

  var url = baseUrl + "/pet/updatePet";

  return await makeHttpRequest(
    url,
    "PUT",
    { "Content-Type": "application/json" },
    200,
    data
  );
};
