export const addPet = async function(
    ownerId,
    petTypeId,
    breedId,
    petName,
    petDob,
    petWeight,
    petOtherInfo,
    vaccines
){
    var url = process.env.REACT_APP_CLIENT_MANAGEMENT_BASE_URL + "/pet";
    const data = {
        ownerId,
        breedId,
        petTypeId,
        name: petName,
        dob: petDob,
        weight: petWeight,
        otherInfo: petOtherInfo,
        vaccines
    };

    return await makeHttpRequest(url, "POST", {"Content-Type": "application/json"}, 201, data);
}