// src/pages/EventDetail.tsx
import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { api } from "../services/api";
import { useAuth } from "../context/AuthContext";
import type { Event, OrderItemInput, OrderResponse } from "../types";

export function EventDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [event, setEvent] = useState<Event | null>(null);
  const [quantities, setQuantities] = useState<Record<number, number>>({});
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    api.get<Event>(`/events/${id}`).then(setEvent);
  }, [id]);

  if (!event) return <p>Loading...</p>;

  const total = event.ticketTypes.reduce(
    (sum, t) => sum + (quantities[t.id] || 0) * t.price,
    0
  );

  async function handleBook() {
    if (!user) {
      navigate("/login");
      return;
    }
    setSubmitting(true);
    setError(null);
    try {
      const items: OrderItemInput[] = Object.entries(quantities)
        .filter(([, qty]) => qty > 0)
        .map(([ticketTypeId, quantity]) => ({ ticketTypeId: Number(ticketTypeId), quantity }));

      if (items.length === 0) {
        setError("Select at least one ticket.");
        return;
      }

      const order = await api.post<OrderResponse>("/orders", {
        eventId: event!.id,
        items,
      });
      navigate(`/orders/${order.orderId}`);
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="p-6 max-w-2xl mx-auto">
      <img src={event.imageUrl} alt={event.title} className="rounded mb-4 w-full h-64 object-cover" />
      <h1 className="text-2xl font-bold">{event.title}</h1>
      <p className="text-gray-500">{event.venueName}, {event.city}</p>
      <p className="text-sm mb-4">{new Date(event.startDateTime).toLocaleString()}</p>
      <p className="mb-6">{event.description}</p>

      <h2 className="font-semibold mb-2">Select tickets</h2>
      {event.ticketTypes.map((t) => (
        <div key={t.id} className="flex justify-between items-center border-b py-2">
          <div>
            <p className="font-medium">{t.name}</p>
            <p className="text-sm text-gray-500">${t.price.toFixed(2)} · {t.available} left</p>
          </div>
          <input
            type="number"
            min={0}
            max={t.available}
            value={quantities[t.id] || 0}
            onChange={(e) =>
              setQuantities({ ...quantities, [t.id]: Number(e.target.value) })
            }
            className="w-16 border rounded px-2 py-1"
          />
        </div>
      ))}

      <p className="mt-4 font-semibold">Total: ${total.toFixed(2)}</p>
      {error && <p className="text-red-600 text-sm mt-2">{error}</p>}

      <button
        onClick={handleBook}
        disabled={submitting}
        className="mt-4 bg-blue-600 text-white px-4 py-2 rounded disabled:opacity-50"
      >
        {submitting ? "Booking..." : "Book Tickets"}
      </button>
    </div>
  );
}