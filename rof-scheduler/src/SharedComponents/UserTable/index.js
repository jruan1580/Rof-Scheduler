import { useState } from 'react';
import { OverlayTrigger, Table, Tooltip } from 'react-bootstrap';
import "./index.css";

function GenericUserTable({ users }){

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
                        <th colSpan={2}></th>
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
                                        <OverlayTrigger placement="top"
                                            overlay={
                                                <Tooltip>
                                                    Lock
                                                </Tooltip>
                                            }
                                        >
                                            <td class="hover-pointer" onClick={() => setShowLockUpdateModal(true)}>
                                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-unlock-fill" viewBox="0 0 16 16">
                                                    <path d="M11 1a2 2 0 0 0-2 2v4a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V9a2 2 0 0 1 2-2h5V3a3 3 0 0 1 6 0v4a.5.5 0 0 1-1 0V3a2 2 0 0 0-2-2z"/>
                                                </svg>
                                            </td>
                                        </OverlayTrigger>           
                                    }                                  
                                    {
                                        user.active && 
                                        <OverlayTrigger placement="top"
                                            overlay={
                                                <Tooltip>
                                                    Inactivate
                                                </Tooltip>
                                            }
                                        >
                                            <td class="hover-pointer" onClick={() => setShowStatusUpdateModal(true)}>
                                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-person-x-fill" viewBox="0 0 16 16">
                                                    <path fill-rule="evenodd" d="M1 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H1zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6zm6.146-2.854a.5.5 0 0 1 .708 0L14 6.293l1.146-1.147a.5.5 0 0 1 .708.708L14.707 7l1.147 1.146a.5.5 0 0 1-.708.708L14 7.707l-1.146 1.147a.5.5 0 0 1-.708-.708L13.293 7l-1.147-1.146a.5.5 0 0 1 0-.708z"/>
                                                </svg>
                                            </td>
                                        </OverlayTrigger>
                                       
                                    }
                                    {
                                        !user.active && 
                                        <OverlayTrigger placement="top"
                                            overlay={
                                                <Tooltip>
                                                    Inactivate
                                                </Tooltip>
                                            }
                                        >
                                            <td class="hover-pointer" onClick={() => setShowStatusUpdateModal(true)}>
                                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-person-check-fill" viewBox="0 0 16 16">
                                                    <path fill-rule="evenodd" d="M15.854 5.146a.5.5 0 0 1 0 .708l-3 3a.5.5 0 0 1-.708 0l-1.5-1.5a.5.5 0 0 1 .708-.708L12.5 7.793l2.646-2.647a.5.5 0 0 1 .708 0z"/>
                                                    <path d="M1 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H1zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6z"/>
                                                </svg>
                                            </td>
                                        </OverlayTrigger>                                       
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