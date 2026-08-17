// src/pages/EventList.tsx
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { api } from "../services/api";
import type { Event } from "../types";

export function EventList() {
  const [events, setEvents] = useState<Event[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api.get<Event[]>("/events").then(setEvents).finally(() => setLoading(false));
  }, []);

  if (loading) return <p>Loading events...</p>;

  return (
    <div className="grid grid-cols-3 gap-4 p-4">
      {events.map((e) => (
        <Link to={`/events/${e.id}`} key={e.id} className="border rounded-lg p-4 hover:shadow-lg">
          <img src={e.imageUrl} alt={e.title} className="rounded mb-2 w-full h-40 object-cover" />
          <h3 className="font-semibold">{e.title}</h3>
          <p className="text-sm text-gray-500">{e.venueName}, {e.city}</p>
          <p className="text-sm">{new Date(e.startDateTime).toLocaleDateString()}</p>
        </Link>
      ))}
    </div>
  );
}