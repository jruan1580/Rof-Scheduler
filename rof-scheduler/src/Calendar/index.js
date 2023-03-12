import FullCalendar from '@fullcalendar/react' // must go before plugins
import dayGridPlugin from '@fullcalendar/daygrid' // a plugin!
import interactionPlugin from "@fullcalendar/interaction" // needed for dayClick
import timeGridPlugin from '@fullcalendar/timegrid'
import { useEffect, useRef, useState } from "react";
import { getAllJobEventsByMonthAndYear } from '../SharedServices/jobEventService';
import { Alert } from "react-bootstrap";
import UpdateEventModal from "./UpdateModal";

function Calendar({setLoginState}) {
  const calendarRef = useRef();
  const [jobEvents, setJobEvents] = useState([]);
  const [errorMessage, setErrorMessage] = useState(undefined);
  const [showUpdateModal, setShowUpdateModal] = useState(false);
  const [updateEvent, setUpdateEvent] = useState(undefined);
  const [schedHour, setSchedHour] = useState("");
  const [schedMin, setSchedMin] = useState("");
  const [schedAMPM, setSchedAMPM] = useState("");
  const [schedDate, setSchedDate] = useState("");

  //loads event for current month once land on page
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

  //clicking on custom buttons to get events for current view
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
            resp = await getAllJobEventsByMonthAndYear(currEndDate.getMonth() + 1, currEndDate.getFullYear());

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

          resp = await getAllJobEventsByMonthAndYear(eventDate.getMonth() + 1, eventDate.getFullYear());

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
  
  //makes list of events set b/w current start and end dates 
  const constructJobEvents = (eventList, jobs) => {      
    for(var i = 0; i < eventList.length; i++){
      jobs.push(eventList[i]);
    }
  };

  //selecting an event
  const handleEventClick = (arg) => {
    getCurrScheduledTime(arg.event);
    setUpdateEvent(arg.event);
    setShowUpdateModal(true);
  }

  //selecting a specific date
  const handleDateSelect = (selectInfo) => {
      alert(selectInfo);
      console.log(selectInfo);
  }

  const getCurrScheduledTime = (event) =>{
    const MM = event.start.getMonth() + 1;
    const dd = event.start.getDate();
    const HH = event.start.getHours();
    const mm = event.start.getMinutes();
    
    let yyyy = event.start.getFullYear();

    var month = (MM < 10) ? "0" + MM : MM;
    var day = (dd < 10) ? "0" + dd : dd;

    var minute = (mm < 10) ? "0" + mm : mm;
    var ampm = (HH < 12) ? "AM" : "PM";
    var date = yyyy + "-" + month + "-" + day
    
    var hour = undefined;

    if(HH < 10){
      hour = "0" + HH;
    }else if(HH > 12){
      var hr = HH - 12;
      hour = "0" + hr;
    }else{
      hour = HH;
    }

    setSchedHour(hour);
    setSchedMin(minute);
    setSchedAMPM(ampm);
    setSchedDate(date);
  }

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
          events = {jobEvents.length !== 0 && 
              jobEvents.map((jobEvent) => {
                  return(
                      { id: jobEvent.id, 
                        title: jobEvent.petServiceName, 
                        start: jobEvent.eventStartTime, 
                        end: jobEvent.eventEndTime,
                        extendedProps: {
                          employeeId: jobEvent.employeeId,
                          employee: jobEvent.employeeFullName,
                          petId: jobEvent.petId,
                          pet: jobEvent.petName,
                          petServiceId: jobEvent.petServiceId,
                          isComplete: jobEvent.completed
                        }
                      }
                  );
              })
          } 
        />

        <UpdateEventModal
          event={updateEvent}
          show={showUpdateModal}
          handleHide={() => setShowUpdateModal(false)}
          setLoginState={setLoginState}
          hour={schedHour}
          minute={schedMin}
          ampm={schedAMPM}
          date={schedDate}
        />
      </>
  )
}

export default Calendar;