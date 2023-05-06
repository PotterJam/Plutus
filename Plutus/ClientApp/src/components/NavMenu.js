import React, { useEffect, useContext } from 'react';
import { Link } from 'react-router-dom';
import  { UserContext } from '../auth/UserContext';
import { FaGithub } from 'react-icons/fa';
import { GoFlame } from 'react-icons/go';
import { getData } from "../Api";

export function NavMenu(props) {
  const { user, loginAttempted, setStreak } = useContext(UserContext);

  const loginWithGithub = `api/login?returnUrl=${window.location.origin}/`;
  const logout = 'api/logout';

  useEffect(() => {
    const getStreak = async () => {
      const resp = await getData("api/streak");
      const streakResp = await resp.json();
      setStreak(streakResp.streakCount);
    }
    
    if (user.authenticated) {
        getStreak();
    }
    }, [user.authenticated]);

  const loginOrProfile = () => {
    return user.authenticated
      ? <div className="inline-block">
          <Link tag={Link} className="font-medium mr-3 text-lg text-slate-800" to="/profile">{user.username}</Link>
          <GoFlame className="mb-1 inline-block" /><span>{user.streak}</span>
        </div>
      : <a
          href={loginWithGithub}
          className="bg-transparent hover:bg-blue-500 text-slate-700 hover:text-white py-2 px-4 border border-slate-500 hover:border-transparent rounded"
          role="button"
        >
          <div className="inline-block">
            <FaGithub className="mb-1 inline-block"/>
            <span className="font-medium">&nbsp; Login</span>
          </div>
        </a>
  }

  return (
      <div className="min-w- sticky top-0 z-40 w-full flex bg-white border-b border-gray-200 inset-x-0 z-100 h-16 items-center">
        <div className="w-full max-w-screen-xl relative mx-auto px-6">
          <div className="flex items-center place-content-between">
            <Link className="flex-none text-slate-800 text-2xl sm:text-3xl" tag={Link} to="/">Plutus</Link>
            <div className="relative flex">
              <div>
                <Link tag={Link} className="font-medium mr-3 text-lg text-slate-800" to="/">Home</Link>
              </div>
              <div className="mx-2">
                {loginAttempted && loginOrProfile()}
              </div>
            </div>
          </div>
      </div>
    </div>
  );
}
