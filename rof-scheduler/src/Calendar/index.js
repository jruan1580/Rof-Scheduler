import FullCalendar from '@fullcalendar/react' // must go before plugins
import dayGridPlugin from '@fullcalendar/daygrid' // a plugin!
import interactionPlugin from "@fullcalendar/interaction" // needed for dayClick
import timeGridPlugin from '@fullcalendar/timegrid'

function Calendar() {
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