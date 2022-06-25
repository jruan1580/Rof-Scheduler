import { OverlayTrigger, Table, Tooltip, Button } from 'react-bootstrap';
import "./index.css";

function GenericUserTable({ users, resetEmployeeLockStatus, updateEmployeeActiveStatus, showUpdateModal }){

    return(
        <>
            <Table responsive striped bordered>
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Primary Phone</th>
                        <th>Role</th>
                        <th>Locked</th>
                        <th>Active</th>
                        <th colSpan={3}></th>
                    </tr>
                </thead>
                <tbody>
                    {
                        users.length != 0 &&
                        users.map((user) => {                    
                            return (
                                <tr key={user.id}>
                                    <td>{user.id}</td>
                                    <td>{user.fullName}</td>
                                    <td>{user.emailAddress}</td>
                                    <td>{user.phoneNumber}</td>
                                    <td>{user.role}</td>
                                    <td>{user.isLocked ? 'Yes' : 'No'}</td>
                                    <td>{user.active ? 'Yes' : 'No'}</td>
                                    {
                                        user.isLocked &&
                                        <td>
                                            <OverlayTrigger placement="top"
                                                overlay={
                                                    <Tooltip>
                                                        Lock
                                                    </Tooltip>
                                                }
                                            >
                                                <Button onClick={() => resetEmployeeLockStatus(user.id)}>                                                
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-unlock-fill" viewBox="0 0 16 16">
                                                        <path d="M11 1a2 2 0 0 0-2 2v4a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V9a2 2 0 0 1 2-2h5V3a3 3 0 0 1 6 0v4a.5.5 0 0 1-1 0V3a2 2 0 0 0-2-2z"/>
                                                    </svg>
                                                </Button>
                                            </OverlayTrigger>        
                                        </td>
                                           
                                    }   
                                    {
                                        !user.isLocked && 
                                        <td>
                                            <OverlayTrigger placement="top"
                                                overlay={
                                                    <Tooltip>
                                                        Lock
                                                    </Tooltip>
                                                }
                                            >
                                                <Button disabled>                                                
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-unlock-fill" viewBox="0 0 16 16">
                                                        <path d="M11 1a2 2 0 0 0-2 2v4a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V9a2 2 0 0 1 2-2h5V3a3 3 0 0 1 6 0v4a.5.5 0 0 1-1 0V3a2 2 0 0 0-2-2z"/>
                                                    </svg>
                                                </Button>
                                            </OverlayTrigger>        
                                        </td>                                      
                                    }                                
                                    {
                                        user.active && 
                                        <td>
                                            <OverlayTrigger placement="top"
                                                overlay={
                                                    <Tooltip>
                                                        Deactivate
                                                    </Tooltip>
                                                }
                                            >
                                                <Button onClick={() => updateEmployeeActiveStatus(user.id, false)}>
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-person-x-fill" viewBox="0 0 16 16">
                                                        <path fillRule="evenodd" d="M1 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H1zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6zm6.146-2.854a.5.5 0 0 1 .708 0L14 6.293l1.146-1.147a.5.5 0 0 1 .708.708L14.707 7l1.147 1.146a.5.5 0 0 1-.708.708L14 7.707l-1.146 1.147a.5.5 0 0 1-.708-.708L13.293 7l-1.147-1.146a.5.5 0 0 1 0-.708z"/>
                                                    </svg>
                                                </Button>
                                            </OverlayTrigger>
                                        </td>                                                                               
                                    }
                                    {
                                        !user.active && 
                                        <td>
                                             <OverlayTrigger placement="top"
                                                overlay={
                                                    <Tooltip>
                                                        Activate
                                                    </Tooltip>
                                                }
                                            >
                                                <Button onClick={() => updateEmployeeActiveStatus(user.id, true)}>
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-person-check-fill" viewBox="0 0 16 16">
                                                        <path fillRule="evenodd" d="M15.854 5.146a.5.5 0 0 1 0 .708l-3 3a.5.5 0 0 1-.708 0l-1.5-1.5a.5.5 0 0 1 .708-.708L12.5 7.793l2.646-2.647a.5.5 0 0 1 .708 0z"/>
                                                        <path d="M1 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H1zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6z"/>
                                                    </svg>
                                                </Button>
                                            </OverlayTrigger>       
                                        </td>                                                                       
                                    }
                                    {
                                        <td>
                                             <OverlayTrigger placement="top"
                                                overlay={
                                                    <Tooltip>
                                                        Update
                                                    </Tooltip>
                                                }
                                            >
                                                <Button onClick={() => showUpdateModal(user)}>
                                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-pencil-fill" viewBox="0 0 16 16">
                                                    <path d="M12.854.146a.5.5 0 0 0-.707 0L10.5 1.793 14.207 5.5l1.647-1.646a.5.5 0 0 0 0-.708l-3-3zm.646 6.061L9.793 2.5 3.293 9H3.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.207l6.5-6.5zm-7.468 7.468A.5.5 0 0 1 6 13.5V13h-.5a.5.5 0 0 1-.5-.5V12h-.5a.5.5 0 0 1-.5-.5V11h-.5a.5.5 0 0 1-.5-.5V10h-.5a.499.499 0 0 1-.175-.032l-.179.178a.5.5 0 0 0-.11.168l-2 5a.5.5 0 0 0 .65.65l5-2a.5.5 0 0 0 .168-.11l.178-.178z"/>
                                                </svg>
                                                </Button>
                                            </OverlayTrigger>     
                                        </td>
                                    }
                                </tr>
                            );
                        })
                    }
                    {
                        users.length == 0 &&
                        <tr>
                            <td colSpan={7} style={{textAlign:"center"}}>No user data available. Please add an user.</td>
                        </tr>
                    }
                </tbody>
            </Table>
        </>
    )
}

export default GenericUserTable;