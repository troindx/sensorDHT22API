/**
 * Function that formats the date in the same way the backend expects.
 * @param date of type Date
 * @returns string formated as Backend expects
 */
export const formatDateToBackend = (date:Date) => {
  // Format the date to "yyyy-MM-ddTHH:mm:ss"
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0'); // Months are 0-based
  const day = String(date.getDate()).padStart(2, '0');
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');
  const seconds = String(date.getSeconds()).padStart(2, '0');
    
  return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`;
};