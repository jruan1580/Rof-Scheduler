import FullCalendar from '@fullcalendar/react' // must go before plugins
import dayGridPlugin from '@fullcalendar/daygrid' // a plugin!
import interactionPlugin from "@fullcalendar/interaction" // needed for dayClick
import timeGridPlugin from '@fullcalendar/timegrid'
import { useEffect, useRef, useState } from "react";
import { GetAllJobEventsByMonthAndYear, GetAllJobEvents } from '../SharedServices/jobEventService';
import { Alert } from "react-bootstrap";

function Calendar({setLoginState}) {
    const calendarRef = useRef();
    const [jobEvents, setJobEvents] = useState([]);
    const [errorMessage, setErrorMessage] = useState(undefined);
    const [eventDate, setEventDate] = useState(new Date());

    useEffect(() => {
    (async function () {
      try {
        setEventDate(calendarRef.current.getApi().getDate());

        var view = calendarRef.current.getApi().view; //grabs current view object for current start and end date

        if(view.type === "timeGridWeek"){
          var jobs = []; //list to hold events from current start date to current end date of view

          //grabs events for month and year of start date
          var currStartDate = view.currentStart;
          var resp = await GetAllJobEventsByMonthAndYear(currStartDate.getMonth() + 1, currStartDate.getFullYear());

          if (resp.status === 401){
            setLoginState(false);
            return;
          }

          //adds event to list
          const startEvents = await resp.json();
          constructJobEvents(startEvents, jobs);

          //grabs events for month and year of end date
          var currEndDate = view.currentEnd;
          var resp = await GetAllJobEventsByMonthAndYear(currEndDate.getMonth() + 1, currEndDate.getFullYear());

          if (resp.status === 401){
            setLoginState(false);
            return;
          }

          //adds events to same list
          const endEvents = await resp.json();
          constructJobEvents(endEvents, jobs);
          
          console.log(eventDate);

          setErrorMessage(undefined);
        }
        
        var resp = await GetAllJobEventsByMonthAndYear(eventDate.getMonth() + 1, eventDate.getFullYear());

        if (resp.status === 401){
          setLoginState(false);
          return;
        }

        const eventList = await resp.json();
        setJobEvents(eventList);
        
        console.log(eventDate);

        setErrorMessage(undefined);
      } catch (e) {
        setErrorMessage(e.message);
      }
    })();
  }, [eventDate]);

  //selecting an event
    const handleEventClick = (arg) => {
        console.log(arg);
        alert(arg);
    }

    //selecting a specific date
    const handleDateSelect = (selectInfo) => {
        alert(selectInfo);
        console.log(selectInfo);
    }

    const constructJobEvents = (eventList, jobs) => {
           
      for(var i = 0; i < eventList.length; i++){
        jobs.push(eventList[i]);
      }

      setJobEvents(jobs);
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
                  left: 'prev,next today',
                  center: 'title',
                  right: 'dayGridMonth,timeGridWeek,timeGridDay'
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
        </>
    )
}

export default Calendar;