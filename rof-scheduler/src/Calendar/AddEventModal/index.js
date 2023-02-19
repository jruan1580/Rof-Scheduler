import { Modal, Form, Row, Col, Button, Spinner, Alert } from "react-bootstrap";
import Select from "react-select";

import { getPetServices } from "../../SharedServices/dropdownService";

function AddEventModal({ show, handleHide, eventDate }) {
  const [employees, setEmployees] = useState([]);
  const [pets, setPets] = useState([]);
  const [petServices, setPetServices] = useState([]);
  const [errorMessage, setErrorMessage] = useState(undefined);

  //load pet services when we land on page
  useEffect(() => {
    (async function () {
      try {
        const resp = await getPetServices();
        if (resp.status === 401) {
          setLoginState(false);
          return;
        }

        const petServices = await resp.json();
        setPetServices(petServices);
      } catch (e) {}
    })();
  }, []);

  //choose pet for event
  const constructPetOptions = (pets) => {
    const petOptions = [];
    for (var i = 0; i < pets.length; i++) {
      petOptions.push({ value: pets[i].id, label: pets[i].name });
    }

    setPets(petOptions);
  };

  //choose employee for event
  const constructEmployeesOption = (employees) => {
    const employeeOptions = [];
    for (var i = 0; i < employees.length; i++) {
      employeeOptions.push({ label: employees[i].fullName, value: employees[i].id });
    }

    setEmployees(employeeOptions);
  };

  const addEventSubmit = (e) => {
    e.preventDefault();

    setErrorMessage(undefined);

    var client = undefined;
    if (localStorage.getItem("role").toLowerCase() != "client") {
      client = parseInt(e.target.client.value);
    }

    var inputValidations = ensureAddPetInformationProvided(
      petName,
      breed,
      weight,
      dob,
      client
    );
    if (inputValidations.size > 0) {
      setValidationMap(inputValidations);
      return;
    }

    setValidationMap(new Map());
    setLoading(true);

    (async function () {
      try {
        //if current user is client, get id from local storage.
        //else its the selected client from dropdown list
        const ownerId =
          localStorage.getItem("role").toLowerCase() !== "client"
            ? client
            : parseInt(localStorage.getItem("id"));
        const resp = await addPet(
          ownerId,
          petTypeSelected,
          breed,
          petName,
          dob,
          weight,
          otherInfo,
          vaccineStatus
        );

        if (resp !== undefined && resp.status === 401) {
          setLoginState(false);
          return;
        }

        setErrMsg(undefined);
        setDisableBtns(true);

        setSuccessMsg(true);

        handleAddPetSuccess();
      } catch (e) {
        setErrMsg(e.message);
        return;
      } finally {
        setLoading(false);
      }
    })();
  };

  //reset everything when we close modal
  const closeModal = function () {
    resetStates();
    handleHide();
  };

  const resetStates = function () {
    setErrorMessage(undefined);
  };

  return (
    <>
      
    </>
  );
}

export default AddEventModal;