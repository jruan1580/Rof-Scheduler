import FullCalendar from '@fullcalendar/react' // must go before plugins
import dayGridPlugin from '@fullcalendar/daygrid' // a plugin!
import interactionPlugin from "@fullcalendar/interaction" // needed for dayClick
import timeGridPlugin from '@fullcalendar/timegrid'
import { useEffect, useRef, useState } from "react";
import { getAllJobEventsByMonthAndYear, addEvent } from '../SharedServices/jobEventService';
import { Alert } from "react-bootstrap";
import AddEventModal from "./AddModal";

import { getPetServices, getPets, getEmployees } from "../SharedServices/dropdownService";
function Calendar({setLoginState}) {
    const calendarRef = useRef();
    const [jobEvents, setJobEvents] = useState([]);
    const [errorMessage, setErrorMessage] = useState(undefined);

    useEffect(() => {
    (async function () {
      try {
        var eventDate = calendarRef.current.getApi().getDate();
        
        var resp = await getAllJobEventsByMonthAndYear(eventDate.getMonth() + 1, eventDate.getFullYear());

        if (resp.status === 401){
          setLoginState(false);
          return;
        }

        const eventList = await resp.json();
        setJobEvents(eventList);

        setErrorMessage(undefined);
      } catch (e) {
        setErrorMessage(e.message);
      }
    })();
  }, []);

  const handleCalendarViewClick = () => {
    (async function () {
      try {
        var view = calendarRef.current.getApi().view; //grabs current view object for current start and end date

        if(view.type === "timeGridWeek"){
          var jobs = []; //list to hold events from current start date to current end date of view

          //grabs events for month and year of start date
          var currStartDate = view.currentStart;
          var resp = await getAllJobEventsByMonthAndYear(currStartDate.getMonth() + 1, currStartDate.getFullYear());

          if (resp.status === 401){
            setLoginState(false);
            return;
          }

          //adds event to list
          const startEvents = await resp.json();
          constructJobEvents(startEvents, jobs);

          //grabs events for month and year of end date
          //only grab events using end date if month is not the same as start date
          //prevents same event being added to the jobs[].
          var currEndDate = view.currentEnd;

          if(currEndDate.getMonth() !== currStartDate.getMonth()){
            var resp = await getAllJobEventsByMonthAndYear(currEndDate.getMonth() + 1, currEndDate.getFullYear());

            if (resp.status === 401){
              setLoginState(false);
              return;
            }

            //adds events to same list
            const endEvents = await resp.json();
            constructJobEvents(endEvents, jobs);
          }
          
          setJobEvents(jobs);
        }else{
          var eventDate = calendarRef.current.getApi().getDate();

          var resp = await getAllJobEventsByMonthAndYear(eventDate.getMonth() + 1, eventDate.getFullYear());

          if (resp.status === 401){
            setLoginState(false);
            return;
          }

          const eventList = await resp.json();
          setJobEvents(eventList);
        }
        setErrorMessage(undefined);
      }catch (e) {
        setErrorMessage(e.message);
      }
    })();
  }

  //selecting an event
    const handleEventClick = (arg) => {
        console.log(arg);
        alert(arg);
    }

  const [showAddModal, setShowAddModal] = useState(false);
  const [employees, setEmployees] = useState([]);
  const [pets, setPets] = useState([]);
  const [petServices, setPetServices] = useState([]);
  const [isMonthView, setIsMonthView] = useState(true);
  const [eventStartDate, setEventStartDate] = useState("");
  const handleDateSelect = (selectInfo) => {
    var view = selectInfo.view;
    if(view.type !== "dayGridMonth"){
      setIsMonthView(false);
    }else{
      setIsMonthView(true);
    }
    
    var eventStart = selectInfo.startStr;
    setEventStartDate(eventStart);
    
    constructEmployeeOptions();
    constructPetOptions();
    constructPetServiceOptions();
    setShowAddModal(true);
  }

    const constructJobEvents = (eventList, jobs) => {
           
      for(var i = 0; i < eventList.length; i++){
        jobs.push(eventList[i]);
      }
    };
  //get employees for dropdown
  const constructEmployeeOptions = () => {
    (async function () {
      try {
        const resp = await getEmployees();
        if (resp.status === 401) {
          setLoginState(false);
          return;
        }

        const employees = await resp.json();
        setEmployees(employees);
        
      } catch (e) {
        setErrorMessage(e.message);
      }
    })();
  }

  //get pet services for dropdown
  const constructPetServiceOptions = () => {
    (async function () {
      try {
        const resp = await getPetServices();
        if (resp.status === 401) {
          setLoginState(false);
          return;
        }

        const petServices = await resp.json();
        setPetServices(petServices);
        
      } catch (e) {
        setErrorMessage(e.message);
      }
    })();
  }

  //get pets for dropdown
  const constructPetOptions = () => {
    (async function () {
      try {
        const resp = await getPets();
        if (resp.status === 401) {
          setLoginState(false);
          return;
        }

        const pets = await resp.json();
        setPets(pets);
        
      } catch (e) {
        setErrorMessage(e.message);
      }
    })();
  };

  const reloadAfterThreeSeconds = () => {
    setTimeout(() => window.location.reload(), 3000);
  };

    return(
        <>
            {errorMessage !== undefined && (
                <Alert variant="danger">{errorMessage}</Alert>
            )}

            <FullCalendar
                ref={calendarRef}
                plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
                headerToolbar={{
                  left: 'prevBtn,nextBtn todayBtn',
                  center: 'title',
                  right: 'toggleMonth,toggleWeek,toggleDay'
                }}
                customButtons={{
                  prevBtn:{
                    text: "<",
                    click: () =>{
                      calendarRef.current.getApi().prev();
                      handleCalendarViewClick();
                    }
                  },
                  nextBtn:{
                    text: ">",
                    click: () => {
                      calendarRef.current.getApi().next();
                      handleCalendarViewClick();
                    }
                  },
                  todayBtn:{
                    text: "today",
                    click: () => {
                      calendarRef.current.getApi().today();
                      handleCalendarViewClick();
                    }
                  },
                  toggleMonth:{
                    text: "month",
                    click: () =>{
                      calendarRef.current.getApi().changeView("dayGridMonth");
                      handleCalendarViewClick();
                    }
                  },
                  toggleWeek:{
                    text: "week",
                    click: () =>{
                      calendarRef.current.getApi().changeView("timeGridWeek");
                      handleCalendarViewClick();
                    }
                  },
                  toggleDay:{
                    text: "day",
                    click: () =>{
                      calendarRef.current.getApi().changeView("timeGridDay");
                      handleCalendarViewClick();
                    }
                  }
                }}
                initialView="dayGridMonth"
                editable={true}
                selectable={true}    
                selectMirror={true}
                select={handleDateSelect}
                eventClick={handleEventClick}
                events = {jobEvents.length != 0 && 
                    jobEvents.map((jobEvent) => {
                        return(
                            { title: jobEvent.petServiceName, start: jobEvent.eventStartTime, end: jobEvent.eventEndTime}
                        );
                    })
                } 
            />

          <AddEventModal 
            show={showAddModal}
            handleHide={() => setShowAddModal(false)}
            handleAddSuccess={reloadAfterThreeSeconds}
            setLoginState={setLoginState}
            eventDate={eventStartDate}
            view={isMonthView}
            employees={employees}
            pets={pets}
            petServices={petServices}
          />
        </>
    )
}

export default Calendar;