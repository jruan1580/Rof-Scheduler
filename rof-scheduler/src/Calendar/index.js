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
        // var resp = await GetAllJobEvents();
        var resp = await GetAllJobEventsByMonthAndYear(eventDate.getMonth() + 1, eventDate.getFullYear());

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
  }, [[eventDate]]);

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

    //clicking prev
    const handlePrevBtn = () => {
      (async function () {
        try {
          var prevDate = eventDate;

          if(eventDate.getMonth() === 0){
            prevDate.setFullYear(eventDate.getFullYear() - 1);
            prevDate.setMonth(11);
          }else{
            prevDate.setMonth(eventDate.getMonth() - 1);
          }

          var resp = await GetAllJobEventsByMonthAndYear(prevDate.getMonth() + 1, prevDate.getFullYear());

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
    }

    const handleNextBtn = () => {
      (async function () {
        try {    
          var nextDate = eventDate;

          if(eventDate.getMonth() === 11){
            nextDate.setFullYear(eventDate.getFullYear() + 1);
            nextDate.setMonth(0);
          }else{
            nextDate.setMonth(eventDate.getMonth() + 1);
          }

          var resp = await GetAllJobEventsByMonthAndYear(nextDate.getMonth() + 1, nextDate.getFullYear());

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
    }

    const handleTodayBtn = () => {
      (async function () {
        try {
          var currDate = new Date();
          eventDate.setMonth(currDate.getMonth());
          eventDate.setFullYear(currDate.getFullYear());

          var resp = await GetAllJobEventsByMonthAndYear(eventDate.getMonth() + 1, eventDate.getFullYear());

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
                  right: 'dayGridMonth,timeGridWeek,timeGridDay'
                }}
                customButtons={{
                  prevBtn: {
                    text: '<',
                    click: () =>{
                      handlePrevBtn();
                      calendarRef.current.getApi().prev();
                    }
                  },
                  nextBtn: {
                    text: '>',
                    click: () => {
                      handleNextBtn();
                      calendarRef.current.getApi().next();
                    }
                  },
                  todayBtn: {
                    text:'today',
                    click: () => {
                      handleTodayBtn();
                      calendarRef.current.getApi().today();
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
        </>
    )
}

export default Calendar;