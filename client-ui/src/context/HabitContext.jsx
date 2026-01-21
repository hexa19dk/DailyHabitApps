import { createContext, useContext, useState, useCallback } from 'react';
import { getCookie } from '../utils/cookieUtils';
import { buildUserProfile, isTokenExpired } from '../utils/tokenUtils';
import api from '../api/axiosInstance';

const HabitContext = createContext();

export const useHabits = () => useContext(HabitContext);

export const HabitProvider = ({ children }) => {
  const [habits, setHabits] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchHabits = useCallback(async () => {
    setLoading(true);
    try {
      const accessToken = getCookie('accessToken');
      const isAuthenticated = accessToken && !isTokenExpired(accessToken);
      if (!isAuthenticated) {
        console.warn('Access token missing or expired. Skipping fetchHabits.');
        setLoading(false);
        return;
      }

      var resToken = buildUserProfile(accessToken);

      // console.log('Fetching habits for user:', resToken.sub);

      const res = await api.get(`/Habit/get-habits/${resToken.sub}`);
      setHabits(res.data || []);
    } catch (err) {
      console.error('Failed to fetch habits:', err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  const createHabit = async (habit) => {
    try {
      const res = await api.post('/Habit/post-habit', habit);

      const createdData = Array.isArray(res.data) ? res.data[0] : res.data;

      setHabits((prev) =>
        Array.isArray(prev)
          ? prev.map((habit) => (habit.id === habit.id ? createdData : habit))
          : [createdData]
      );

      return res;
    } catch (err) {
      console.error('Create habit error:', err);
      if (err.code === 'ERR_NETWORK') {
        console.error('Network error: Unable to connect to the server. Please check if the API server is running.');
      } else if (err.response?.status === 0) {
        console.error('CORS error: The server is not allowing cross-origin requests. Please check the server CORS configuration.');
      } else {
        console.error(`Failed to create habit: ${err.response?.data?.message || err.message}`);
      }
      throw err; // Re-throw to let the calling component handle it
    }
  };

  const updateHabit = async (id, updatedHabit) => {
    try {
      const res = await api.put(`/Habit/update-habit/${id}`, updatedHabit);

      const updatedData = Array.isArray(res.data) ? res.data[0] : res.data;

      setHabits((prev) =>
        Array.isArray(prev)
          ? prev.map((habit) => (habit.id === id ? updatedData : habit))
          : [updatedData]
      );

      return res;
    } catch (err) {
      console.error('Failed to update habit' + err.message);
    }
  };

  const deleteHabit = async (id) => {
    try {
      // 1. Delete the habit from the server
      const res = await api.delete(`/Habit/delete-habit/${id}`);

      // 2. After successful deletion, fetch the updated habits
      const accessToken = getCookie('accessToken');
      const resToken = buildUserProfile(accessToken);
      const habitsRes = await api.get(`/Habit/get-habits/${resToken.userId}`);

      // 3. Update the state with fresh data from the server
      setHabits(habitsRes.data || []);

      return res;
    } catch (err) {
      console.error('Failed to delete habit: ' + err.message);
      throw err; // Re-throw to let the calling component handle the error
    }
  }

  return (
    <HabitContext.Provider value={{ habits, loading, fetchHabits, createHabit, updateHabit, deleteHabit }}>
      {children}
    </HabitContext.Provider>
  );
};