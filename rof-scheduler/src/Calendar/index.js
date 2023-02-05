import FullCalendar from '@fullcalendar/react' // must go before plugins
import dayGridPlugin from '@fullcalendar/daygrid' // a plugin!
import interactionPlugin from "@fullcalendar/interaction" // needed for dayClick
import timeGridPlugin from '@fullcalendar/timegrid'
import { useEffect, useState } from "react";
import { GetAllJobEventsByMonthAndYear } from '../SharedServices/jobEventService';
import { Alert } from "react-bootstrap";

function Calendar({setLoginState}) {
    const [jobEvents, setJobEvents] = useState([]);
    const [errorMessage, setErrorMessage] = useState(undefined);
    const [eventDate, setEventDate] = useState(new Date());

    useEffect(() => {
    (async function () {
      try {
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
  }, [eventDate]);

    const handleEventClick = (arg) => {
        console.log(arg);
        alert(arg);
    }

    const handleDateSelect = (selectInfo) => {
        alert(selectInfo);
        console.log(selectInfo);
    }

    return(
        <>
            {errorMessage !== undefined && (
                <Alert variant="danger">{errorMessage}</Alert>
            )}

            <FullCalendar
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