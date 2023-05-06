import React, { useContext, useEffect, useState } from 'react';
import { FiLogOut } from 'react-icons/fi';
import { getData, postData } from "../Api";
import { UserContext } from '../auth/UserContext';
import { BsSpotify } from 'react-icons/bs';

export function Profile() {
  
  const { user } = useContext(UserContext);
  
  const [username, setUsername] = useState("");
  const logout = 'api/logout';
  
  return (
    <div className="h-max mx-auto border border-slate-300 bg-white w-max mt-10">
      <div className='flex flex-col items-center px-20 py-10'>
        <span>Logout</span>
        <a href={logout}><FiLogOut className="ml-2 mb-1 inline-block"/></a>
      </div>
    </div>
  );
}
