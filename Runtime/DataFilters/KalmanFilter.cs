using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
	* @brief  KalmanFilter is a float value which is filtered with a KalmanFilter.
	* Usage:
	*
	*    var k = new KalmanFilter(start_value)
	*    k.push(new_value_1)
	*    k.push(new_value_2)
	*    var filtered_value = k.get()
	*
	* @author John Hardy
	*/
public class KalmanFilter
{
	private float A = 1.0F;	// Factor of real value to previous real value
	private float B = 0.0F;	// Factor of real value to real control signal
	private float H = 1.0F;	// Factor of measured value to real value
	public float smoothingFactor = 0.02F;
	
	private float P = 1.0F;		// 
	public float noise = 0.4F;	// Enviromental noise.

	private float value = 0.0F;	// The value .
	private float last = 0.0F;		// The last value.

	private float LP;
	
	/**
		* @brief Create a new Kalman filtered double.
		* @param fStartingValue The starting value for the filter.
		*/
	public KalmanFilter(float fStartingValue = 0, float _smoothingFactor = -1)
	{
        if (_smoothingFactor != -1)
            smoothingFactor = _smoothingFactor;

        this.Reset(fStartingValue);
	}
	
	/**
		* @brief Reset this Kalman filtered double.
		* @param fStartingValue The starting value for the filter.
		*/
	public void Reset(float fStartingValue)
	{
		this.LP = 0.1F;
		this.value = fStartingValue;
		this.last = fStartingValue;
	}
	
	/**
		* @brief Push a value into the filter to modify it by it.
		* @param fValue The value to filter.
		*/
	public void Push(float fValue)
	{
		// time update - prediction
		this.last = this.A * this.last;
		this.LP = this.A * this.LP * this.A + this.smoothingFactor;
		
		// measurement update - correction
		var K = this.LP * this.H / (this.H * this.LP * this.H + this.noise);
		this.last = this.last + K * (fValue - this.H * this.last);
		this.LP = (1.0F - K * this.H) * this.LP;
		
		// Store the update.
		this.value = this.last;
	}
	
	/**
		* @brief Return the current filtered value.
		*/
	public float Get()
	{
		return this.value;
	}
}