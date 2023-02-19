import FullCalendar from '@fullcalendar/react' // must go before plugins
import dayGridPlugin from '@fullcalendar/daygrid' // a plugin!
import interactionPlugin from "@fullcalendar/interaction" // needed for dayClick
import timeGridPlugin from '@fullcalendar/timegrid'
import { useState, useRef } from "react";

function Calendar() {
  const handleEventClick = (arg) => {
      console.log(arg);
      alert(arg);
  }

  const handleDateSelect = (selectInfo) => {
    //employeeId, petId, petServiceId, eventStartTime <- needed for add

    //month view time is 00:00, need to get time.

    //if(view = month){
      //alert to select time
      //set start time
    // }else{
      //start time = time selected
    // }

    //grab employee, pet, pet service
    //alert to chose
    //set ids

    //add event
    
    console.log(selectInfo.start);
      alert(selectInfo);
  }

    // const getEmployees = () => {
    //   (async function() {
    //   try {
    //     //need a get Employee drop down
    //   } catch (e) {
    //     setErrorMessage(e.message);
    //   }
    // })
    // }

    // const getPets = () => {
    //   (async function() {
    //   try {
    //     //need a get Pet drop down
    //   } catch (e) {
    //     setErrorMessage(e.message);
    //   }
    // })
    // }

    const getPetServices = () => {
      (async function() {
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
      })
    }

    return(
        <>
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
                events={[
                    { title: 'event 1', start: '2022-04-29T05:00:00', end: '2022-04-29T07:00:00'},
                    { title: 'event 2', start: '2022-04-29T05:00:00', end: '2022-04-29T06:00:00' }
                  ]}
            />
        </>
    )
}

export default Calendar;