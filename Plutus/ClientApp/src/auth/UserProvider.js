import { useState } from "react";
import { UserContext } from "./UserContext";

export const UserProvider = ({ children }) => {
    // User is the name of the "data" that gets stored in context
    const [user, setUser] = useState({ username: '', authenticated: null });

    const loginAttempted = user.authenticated !== null;
  
    const login = name => {
      setUser(_ => ({
        username: name,
        authenticated: true
      }));
    };
  
    const logout = () => {
      setUser(_ => ({
        username: '',
        authenticated: false
      }));
    };
    
    return (
      <UserContext.Provider value={{ user, loginAttempted, login, logout }}>
        {children}
      </UserContext.Provider>
    );
  }
  