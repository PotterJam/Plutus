import React, { useContext, useEffect, useState } from 'react';
import { getData, postData } from "../Api";
import { UserContext } from '../auth/UserContext';
import { parseISO } from 'date-fns'
import { BiMusic } from 'react-icons/bi';

export function Home() {
  
  const { user, loginAttempted } = useContext(UserContext);
  
  useEffect(() => {
    if (user.authenticated) {
        // Fetch and populate data
    }
  }, [user.authenticated]);

  const aboutElement = (
    <div className='flex flex-col w-4/5 sm:w-auto rounded bg-white border p-12'>
      <h1 className='font-serif-header font-bold text-4xl sm:text-5xl text-slate-800 mb-8'>Plutus</h1>
      <span className='pb-3 text-xl'>This is a work in progress.</span>
    </div>
  );

  return (
    <div className="h-full flex mt-4 p-0.5 sm:p-2 flex-col items-center">
      {loginAttempted && user.authenticated && aboutElement}
      {loginAttempted && !user.authenticated && aboutElement}
    </div>
  );
}
